using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{

    public WorldSettings worldSettings;
    public WorldRenderer worldRenderer;
    public TerrainGenerator terrainGenerator;
    public UnityEvent OnWorldCreated, OnNewChunksGenerated;

    

    public struct WorldGenerationData{
        public List<Vector3Int> chunkPositionsToCreate;
        public List<Vector3Int> chunkDataPositionsToCreate;
        public List<Vector3Int> chunkPositionsToRemove;
        public List<Vector3Int> chunkDataToRemove;
    };

    public struct WorldData{
        public ConcurrentDictionary<Vector3Int, ChunkData> chunkDataDictionary;
        public ConcurrentDictionary<Vector3Int, ChunkRenderer> chunkDictionary;
        public ConcurrentQueue<MeshData> meshDataQueue;
        public ConcurrentQueue<Vector3> loadPositionsQueue;


        public WorldSettings worldSettings;
    };

    public WorldData worldData { get; private set; }
    public bool IsWorldCreated { get; private set; }

    CancellationTokenSource taskTokenSource = new CancellationTokenSource();

    private WorldGenerationData GetWorldGenerationDataAroundPlayer(Vector3Int playerPosition){
        List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPosition);
        List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPosition);


        WorldGenerationData data = new WorldGenerationData
        {
            chunkPositionsToCreate = WorldDataHelper.SelectPositonsToCreate(worldData, allChunkPositionsNeeded, playerPosition),
            chunkDataPositionsToCreate = WorldDataHelper.SelectDataPositonsToCreate(worldData, allChunkDataPositionsNeeded, playerPosition),
            chunkPositionsToRemove = WorldDataHelper.GetUnnededChunks(worldData, allChunkPositionsNeeded),
            chunkDataToRemove = WorldDataHelper.GetUnnededData(worldData, allChunkDataPositionsNeeded)
        };

        return data;
    }

    private void Awake(){
        worldData = new WorldData{
            worldSettings = this.worldSettings,
            chunkDataDictionary = new ConcurrentDictionary<Vector3Int, ChunkData>(),
            chunkDictionary = new ConcurrentDictionary<Vector3Int, ChunkRenderer>(),
            meshDataQueue = new ConcurrentQueue<MeshData>(),
            loadPositionsQueue = new ConcurrentQueue<Vector3>()

        };
        
        worldData.loadPositionsQueue.Enqueue(worldSettings.startingPosition);

        IsWorldCreated = false;

        worldSettings.actualChunkSize = Vector3.Scale(worldSettings.chunkSize, worldSettings.voxelSize);

        CalculateVoxelSizeInverse();

        worldSettings.minMapDimensions = Vector3.Scale(worldSettings.voxelMinMapDimensions, worldSettings.voxelSize);
        worldSettings.maxMapDimensions = Vector3.Scale(worldSettings.voxelMaxMapDimensions, worldSettings.voxelSize);
    }

    bool isFirstWorldDataReady = false;
    Queue<Task> generateWorldTaskQueue = new Queue<Task>();

    void Update()
    {
        if(isFirstWorldDataReady && !IsWorldCreated){

            StartCoroutine(ChunkCreationCoroutine());
            IsWorldCreated = true;
            OnWorldCreated?.Invoke();
        }

        if(generateWorldTaskQueue.Count > 0){
            Task generateWorldTask = generateWorldTaskQueue.Peek();
            if(generateWorldTask.IsCompletedSuccessfully){
                generateWorldTask = generateWorldTaskQueue.Dequeue();
                generateWorldTask.Dispose();
            }
        }
    }
    
    public void GenerateWorld()
    {
        IsWorldCreated = false;
        
        
        StartGenerateWorldTask(worldSettings.startingPosition);
        
    }

    public void StartGenerateWorldTask(Vector3 position){
        Task generateWorldTask = new Task(() => GenerateWorldTask(position));
        generateWorldTask.Start();

        generateWorldTaskQueue.Enqueue(generateWorldTask);
    }

    private void GenerateWorldTask(Vector3 position)
    {
        
        GenerateWorld(Vector3Int.RoundToInt(position));
        

        if(isFirstWorldDataReady == false){
            isFirstWorldDataReady = true;
        }

        
    }

    public void DeleteWorld(){

        List<Vector3Int> worldChunkData = new List<Vector3Int>(worldData.chunkDataDictionary.Keys);
        List<Vector3Int> worldChunk = new List<Vector3Int>(worldData.chunkDictionary.Keys);
    
        foreach(var pos in worldChunk){
            WorldDataHelper.RemoveChunk(this, pos);
        }

        foreach(var pos in worldChunkData){
            WorldDataHelper.RemoveChunkData(this, pos);
        }
    
    }
    

    private void GenerateWorld(Vector3Int position){
        
        WorldGenerationData worldGenerationData = GetWorldGenerationDataAroundPlayer(position);

        foreach(var pos in worldGenerationData.chunkPositionsToRemove){
            WorldDataHelper.RemoveChunk(this, pos);
        }

        foreach(var pos in worldGenerationData.chunkDataToRemove){
            WorldDataHelper.RemoveChunkData(this, pos);
        }

        var watch = System.Diagnostics.Stopwatch.StartNew();
        ConcurrentDictionary<Vector3Int, ChunkData> dataDictionary = null;
        
        
        dataDictionary = CalculateWorldChunkData(worldGenerationData.chunkDataPositionsToCreate);
        

        foreach (var calculatedData in dataDictionary)
        {
            worldData.chunkDataDictionary.TryAdd(calculatedData.Key, calculatedData.Value);
        }

        Parallel.ForEach(worldData.chunkDataDictionary.Values,  chunkData  => {
            AddOutOfChunkBoundsVoxel(chunkData);
        });

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("WorldData " + elapsedMs);

        watch = System.Diagnostics.Stopwatch.StartNew();

        List<ChunkData> dataToRender = worldData.chunkDataDictionary
            .Where(keyvaluepair => worldGenerationData.chunkPositionsToCreate.Contains(keyvaluepair.Key))
            .Select(keyvalpair => keyvalpair.Value)
            .ToList();

        
        CreateMeshData(dataToRender);
        
        watch.Stop();
        elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("MeshData " + elapsedMs);

        
        
    }


    IEnumerator ChunkCreationCoroutine() 
    {
        while(true){
            yield return meshDataToChunkRenderer();

            if(IsWorldCreated == false)
            {
                IsWorldCreated = true;
                OnWorldCreated?.Invoke();
            }
            else{
                OnNewChunksGenerated?.Invoke();
            }

            yield return null;
        }

    }

    IEnumerator meshDataToChunkRenderer(){
        var watch = System.Diagnostics.Stopwatch.StartNew();
        while(!worldData.meshDataQueue.IsEmpty)
        {
            
            MeshData meshData;
            if(!worldData.meshDataQueue.TryDequeue(out meshData)){
                break;
            }

            Vector3Int position = meshData.worldPosition;

            if(!worldData.chunkDataDictionary.ContainsKey(position)){
                continue;
            }

            ChunkRenderer chunkRenderer = worldRenderer.RenderChunk(worldData.chunkDataDictionary[position], position, meshData);

            worldData.chunkDictionary.TryAdd(position, chunkRenderer);

            yield return new WaitForEndOfFrame();
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        
    }

    private void AddOutOfChunkBoundsVoxel(ChunkData chunkData)
    {
        
        Dictionary<Vector3Int, VoxelType> placedVoxels = new Dictionary<Vector3Int, VoxelType>();

        foreach(KeyValuePair<Vector3Int, VoxelType> outOfChunkBoundsVoxel in chunkData.outOfChunkBoundsVoxelDictionary){
            
            Vector3Int worldPosition = outOfChunkBoundsVoxel.Key;
            VoxelType voxelType = outOfChunkBoundsVoxel.Value;

            if(WorldDataHelper.SetVoxelFromWorldCoordinates(this, worldPosition, voxelType)){
                placedVoxels.Add(worldPosition, voxelType);
            }

            

        }

        chunkData.outOfChunkBoundsVoxelDictionary = 
                chunkData.outOfChunkBoundsVoxelDictionary.Where(x => placedVoxels.ContainsKey(x.Key) == false)
                .ToDictionary(x => x.Key, x => x.Value);

        
    }

   
    

    ConcurrentDictionary<Vector3Int, ChunkData> CalculateWorldChunkData(List<Vector3Int> chunkDataPositionsToCreate){

        ConcurrentDictionary<Vector3Int, ChunkData> dictionary = new ConcurrentDictionary<Vector3Int, ChunkData>();
     
        Parallel.ForEach(chunkDataPositionsToCreate, pos =>
        {
            if (taskTokenSource.Token.IsCancellationRequested)
            {
                taskTokenSource.Token.ThrowIfCancellationRequested();
            }
            ChunkData data = new ChunkData(worldData.worldSettings.chunkSize, this, pos);
            ChunkData newData = terrainGenerator.GenerateChunkData(data);
            dictionary.TryAdd(pos, newData);
        });
        return dictionary;
        

    
    }

    void CreateMeshData(List<ChunkData> dataToRender){

            
        Parallel.ForEach(dataToRender, data =>
        {

            if(taskTokenSource.Token.IsCancellationRequested){
                taskTokenSource.Token.ThrowIfCancellationRequested();
            }

            MeshData meshData = Chunk.GetChunkMeshData(data);
            meshData.worldPosition = data.worldPosition;
            worldData.meshDataQueue.Enqueue(meshData);

        });


    }

    public bool SetVoxel(RaycastHit hit, VoxelType voxelType){

        ChunkRenderer chunk = hit.transform.parent.GetComponent<ChunkRenderer>();
        
        if(chunk == null){
            return false;
        }



        Vector3Int worldPos = GetBlockPosition(hit);

        WorldDataHelper.SetVoxelFromWorldCoordinates(chunk.ChunkData.worldReference, worldPos, voxelType);
        chunk.modifiedByPlayer = true;

        CheckEdges(chunk.ChunkData, worldPos);

        chunk.UpdateChunk();

        return true;


    }

    internal VoxelType CheckVoxel(RaycastHit hit)
    {
        ChunkRenderer chunk = hit.transform.parent.GetComponent<ChunkRenderer>();
        

        if(chunk == null){
            return VoxelType.Nothing;
        }
        
        Vector3Int worldPos = GetBlockPosition(hit);
        return WorldDataHelper.GetVoxelFromWorldCoorinates(chunk.ChunkData.worldReference, worldPos);

    }
    internal VoxelType CheckVoxel(Vector3 pos)
    {
        
        Vector3Int worldPos = GetBlockPosition(pos);

        return WorldDataHelper.GetVoxelFromWorldCoorinates(this, worldPos);

    }

    private void CheckEdges(ChunkData chunkData, Vector3Int worldPos){

        if(!Chunk.VoxelIsOnEdge(chunkData, worldPos)){
            return;
        }

        List<ChunkData> neighbourDataList = Chunk.GetEdgeNeighbourChunk(chunkData, worldPos);

        foreach(var neighbourData in neighbourDataList){
            
            if(neighbourData == null){
                continue;
            }
            
            ChunkRenderer chunkToUpdate = WorldDataHelper.GetChunk(neighbourData.worldReference, neighbourData.worldPosition);

            if(chunkToUpdate != null){
                chunkToUpdate.UpdateChunk();
            }

        }

    }

    private Vector3Int GetBlockPosition(RaycastHit hit){

        Vector3 hitPos = Vector3.Scale(hit.point, this.worldSettings.inverseVoxelSize);
        Vector3 hitNormal = Vector3.Scale(hit.normal, this.worldSettings.inverseVoxelSize);

        Vector3 pos = new Vector3(
            GetRealPosition(hitPos.x, hitNormal.x),
            GetRealPosition(hitPos.y, hitNormal.y),
            GetRealPosition(hitPos.z, hitNormal.z)
        );

        return Vector3Int.RoundToInt(pos);

    }

    private Vector3Int GetBlockPosition(Vector3 pos){

        pos = Vector3.Scale(pos, this.worldSettings.inverseVoxelSize);

        return Vector3Int.RoundToInt(pos);

    }

    private float GetRealPosition(float pos, float normal){

        return (float)((Mathf.Abs(pos % 1) == 0.5f) ? pos - normal/2 : pos);

    }

    public void CalculateVoxelSizeInverse(){
        worldSettings.inverseVoxelSize = new Vector3(
            1f / worldSettings.voxelSize.x,
            1f / worldSettings.voxelSize.y,
            1f / worldSettings.voxelSize.z
        );
    }

    public void OnDisable(){

        taskTokenSource.Cancel();

    }

    
}
