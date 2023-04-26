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

    public Vector3Int startingPosition = new Vector3Int(0, 0, 0);

    public struct WorldGenerationData{
        public List<Vector3Int> chunkPositionsToCreate;
        public List<Vector3Int> chunkDataPositionsToCreate;
        public List<Vector3Int> chunkPositionsToRemove;
        public List<Vector3Int> chunkDataToRemove;
    };

    public struct WorldData{
        public ConcurrentDictionary<Vector3Int, ChunkData> chunkDataDictionary;
        public ConcurrentDictionary<Vector3Int, ChunkRenderer> chunkDictionary;
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
            chunkDictionary = new ConcurrentDictionary<Vector3Int, ChunkRenderer>()
        };
        IsWorldCreated = false;
    }

    public async void GenerateWorld(){

        IsWorldCreated = false;
        await GenerateWorld(startingPosition);
        
    }

    void Update(){
        
        if(Input.GetKeyDown(KeyCode.R)){

            List<Vector3Int> chunkList = new List<Vector3Int>(worldData.chunkDictionary.Keys);
            List<Vector3Int> chunkDataList = new List<Vector3Int>(worldData.chunkDataDictionary.Keys);
            

            foreach(var pos in chunkList){
                WorldDataHelper.RemoveChunk(this, pos);
            }

            foreach(var pos in chunkDataList){
                WorldDataHelper.RemoveChunkData(this, pos);
            }   

            SceneManager.LoadScene("VoxelWorld");
        
        }
    }
    

    private async Task GenerateWorld(Vector3Int position){

        WorldGenerationData worldGenerationData = GetWorldGenerationDataAroundPlayer(position);

        foreach(var pos in worldGenerationData.chunkPositionsToRemove){
            WorldDataHelper.RemoveChunk(this, pos);
        }

        foreach(var pos in worldGenerationData.chunkDataToRemove){
            WorldDataHelper.RemoveChunkData(this, pos);
        }

    
        ConcurrentDictionary<Vector3Int, ChunkData> dataDictionary = null;

        try
        {
            dataDictionary = await CalculateWorldChunkData(worldGenerationData.chunkDataPositionsToCreate);
        }
        catch (Exception ex)
        {
            Debug.Log("Task cancelled");
            Debug.Log(ex.Message);
            return;
        }

        foreach (var calculatedData in dataDictionary)
        {
            if(worldData.chunkDataDictionary.ContainsKey(calculatedData.Key)){
                WorldDataHelper.RemoveChunkData(this, calculatedData.Key);
            }
            
            worldData.chunkDataDictionary.TryAdd(calculatedData.Key, calculatedData.Value);
        }

        foreach(ChunkData chunkData in worldData.chunkDataDictionary.Values){
            AddOutOfChunkBoundsVoxel(chunkData);
        }
    
        ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary = new ConcurrentDictionary<Vector3Int, MeshData>();

        List<ChunkData> dataToRender = worldData.chunkDataDictionary
            .Where(keyvaluepair => worldGenerationData.chunkPositionsToCreate.Contains(keyvaluepair.Key))
            .Select(keyvalpair => keyvalpair.Value)
            .ToList();

        try{
            meshDataDictionary = await CreateMeshData(dataToRender);
        }
        catch(Exception ex){
            Debug.Log("Task cancelled");
            Debug.Log(ex.Message);
            return;
        }

        StartCoroutine(ChunkCreationCoroutine(meshDataDictionary));
        /*
        foreach(var item in meshDataDictionary){

            Vector3Int pos =  item.Key;
            MeshData meshData = item.Value;

            ChunkRenderer chunkRenderer = worldRenderer.RenderChunk(worldData.chunkDataDictionary[pos], pos, meshData);

            if(worldData.chunkDictionary.ContainsKey(pos)){
                WorldDataHelper.RemoveChunk(this, pos);
            }
            
            worldData.chunkDictionary.Add(pos, chunkRenderer);
            
                

        }

        if(IsWorldCreated == false){
            IsWorldCreated = true;
            OnWorldCreated?.Invoke();
        }

        */

    }

    

    IEnumerator ChunkCreationCoroutine(ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary) 
    {
        foreach (var item in meshDataDictionary)
        {
            Vector3Int position = item.Key;
            MeshData meshData = item.Value;

            if(!worldData.chunkDataDictionary.ContainsKey(position)){
                continue;
            }

            ChunkRenderer chunkRenderer = worldRenderer.RenderChunk(worldData.chunkDataDictionary[position], position, meshData);
        
            if(worldData.chunkDictionary.ContainsKey(position)){
                WorldDataHelper.RemoveChunkData(this, position);
            }

            worldData.chunkDictionary.TryAdd(position, chunkRenderer);

            yield return null;
        }
        if (IsWorldCreated == false)
        {
            IsWorldCreated = true;
            OnWorldCreated?.Invoke();
        }
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
            
            Debug.Log(chunkData.outOfChunkBoundsVoxelDictionary.ToString());
            Debug.Log(placedVoxels.ToString());

            chunkData.outOfChunkBoundsVoxelDictionary = 
                chunkData.outOfChunkBoundsVoxelDictionary.Where(x => placedVoxels.ContainsKey(x.Key) == false)
                .ToDictionary(x => x.Key, x => x.Value);

            Debug.Log(chunkData.outOfChunkBoundsVoxelDictionary.ToString());
            Debug.Log("");
            Debug.Log("");

        }

        
    }

   
    

    private Task<ConcurrentDictionary<Vector3Int, ChunkData>> CalculateWorldChunkData(List<Vector3Int> chunkDataPositionsToCreate){

        ConcurrentDictionary<Vector3Int, ChunkData> dictionary = new ConcurrentDictionary<Vector3Int, ChunkData>();

        return Task.Run(() => 
        {
            foreach (Vector3Int pos in chunkDataPositionsToCreate)
            {
                if (taskTokenSource.Token.IsCancellationRequested)
                {
                    taskTokenSource.Token.ThrowIfCancellationRequested();
                }
                ChunkData data = new ChunkData(worldData.worldSettings.chunkSize, this, pos);
                ChunkData newData = terrainGenerator.GenerateChunkData(data);
                dictionary.TryAdd(pos, newData);
            }
            return dictionary;
        },
        taskTokenSource.Token
        );

    }

    private Task<ConcurrentDictionary<Vector3Int, MeshData>> CreateMeshData(List<ChunkData> dataToRender){


        ConcurrentDictionary<Vector3Int, MeshData> meshDictionary = new ConcurrentDictionary<Vector3Int, MeshData>();
        
        return Task.Run(() =>
        {
            foreach(ChunkData data in dataToRender){

                if(taskTokenSource.Token.IsCancellationRequested){
                    taskTokenSource.Token.ThrowIfCancellationRequested();
                }

                MeshData meshData = Chunk.GetChunkMeshData(data);
                meshDictionary.TryAdd(data.worldPosition, meshData);

            }

            return meshDictionary;
        },
        taskTokenSource.Token
        );

    }

    public async void LoadAdditionalChunksRequest(GameObject player){
        Debug.Log("Requesting more chunks");
        await GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
        OnNewChunksGenerated?.Invoke();
    }

    public bool SetVoxel(RaycastHit hit, VoxelType voxelType){

        ChunkRenderer chunk = hit.collider.GetComponent<ChunkRenderer>();
        
        if(chunk == null){
            return false;
        }

        Vector3Int worldPos = GetBlockPosition(hit);
        Debug.Log(worldPos);

        WorldDataHelper.SetVoxelFromWorldCoordinates(chunk.ChunkData.worldReference, worldPos, voxelType);
        chunk.modifiedByPlayer = true;

        CheckEdges(chunk.ChunkData, worldPos);

        chunk.UpdateChunk();

        return true;


    }

    internal VoxelType CheckVoxel(RaycastHit hit)
    {
        ChunkRenderer chunk = hit.collider.GetComponent<ChunkRenderer>();
        

        if(chunk == null){
            return VoxelType.Nothing;
        }
        
        Vector3Int worldPos = GetBlockPosition(hit);
        Debug.Log(worldPos);
        return WorldDataHelper.GetVoxelFromWorldCoorinates(chunk.ChunkData.worldReference, worldPos);

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

        Vector3 pos = new Vector3(
            GetRealPosition(hit.point.x, hit.normal.x),
            GetRealPosition(hit.point.y, hit.normal.y),
            GetRealPosition(hit.point.z, hit.normal.z)
        );

        return Vector3Int.RoundToInt(pos);

    }

    private float GetRealPosition(float pos, float normal){

        return (float)((Mathf.Abs(pos % 1) == 0.5f) ? pos - normal/2 : pos);

    }

    public void OnDisable(){

        taskTokenSource.Cancel();

    }

    
}
