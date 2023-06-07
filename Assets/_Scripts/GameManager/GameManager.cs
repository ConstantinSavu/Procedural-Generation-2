using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Unity.AI;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player;

    public GameObject[] enemyPrefabs;
    private GameObject enemy;
    
    public GameObject RenderedChunks;
    public Vector3Int currentPlayerChunkPosition;
    public Vector3Int currentChunkCenter = Vector3Int.zero;

    public bool loadAdditionalChunks = false;

    public World world;

    public float detectionTime = 0.5f;
    public CinemachineVirtualCamera camera_VM;

    private NavMeshSurface[] surfaces;

    public void Awake(){

        if(RenderedChunks == null){

            Debug.Log("Rendered Chunks is null or dosen't have Navmesh Component, pls fix");

        }
        
        camera_VM.transform.position = new Vector3(0, world.worldSettings.voxelMaxMapDimensions.y, 0);
        camera_VM.transform.rotation *= Quaternion.Euler(Vector3.right * 90);
        camera_VM.m_Lens.ModeOverride = Cinemachine.LensSettings.OverrideModes.Orthographic;
    
    }

    public void Update(){

        if(player == null){
            return;
        }

        if(Input.GetKeyDown(KeyCode.B)){
            CreateNavMeshes();
        }

    }

    public void SpawnPlayer(){

        if (player != null)
            return;
        Vector3 raycastStartposition = world.worldSettings.startingPosition;
        raycastStartposition.y = world.worldSettings.maxMapDimensions.y;

        float rayCastLength = world.worldSettings.maxMapDimensions.y - world.worldSettings.minMapDimensions.y + 30;

        RaycastHit hit;

        CreateNavMeshes();
        
        if (Physics.Raycast(raycastStartposition, Vector3.down, out hit, rayCastLength))
        {
            
            SpawnPlayer(hit);
            StartCheckingTheMap();
            SpawnEnemy(player);
            

            
            return;
        }

        Debug.Log("Player not spawned");

        SpawnPlayer(world.worldSettings.maxMapDimensions);
        StartCheckingTheMap();
        
        
    }

    private void SpawnPlayer(RaycastHit hit){
        player = Instantiate(playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
        PlayerCamera playerCamera = player.GetComponentInChildren<PlayerCamera>();
        playerCamera.SetCamera(camera_VM);
    }

    private void SpawnPlayer(Vector3 spawnPosition){
        player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        PlayerCamera playerCamera = player.GetComponentInChildren<PlayerCamera>();
        playerCamera.SetCamera(camera_VM);
    }

    private void SpawnEnemy(GameObject player){

        Transform target = player.transform.Find("TargetForEnemies");

        enemy = Instantiate(enemyPrefabs[0], player.transform.position + Vector3.back , Quaternion.identity);
        enemy.GetComponentInChildren<Enemy>().InstantiateEnemy(target);

        enemy = Instantiate(enemyPrefabs[1], player.transform.position + Vector3.forward , Quaternion.identity);
        enemy.GetComponentInChildren<Enemy>().InstantiateEnemy(target);
        
    }

    private void CreateNavMeshes()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        
        surfaces = RenderedChunks.GetComponentsInChildren<NavMeshSurface>();

        foreach(NavMeshSurface surface in surfaces){
            surface.RemoveData();
            surface.BuildNavMesh();
            
            surface.AddData();
        }
        
        
        
        

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("NavMesh " + elapsedMs);
    }

    public void StartCheckingTheMap(){
        SetCurrentChunkCoordinates();
        StopAllCoroutines();
        StartCoroutine(CheckIfShouldLoadNextPosition());
    }
    
    private void SetCurrentChunkCoordinates(){
        currentPlayerChunkPosition = WorldDataHelper.ChunkPositionFromVoxelCoords(world, Vector3Int.RoundToInt(player.transform.position));

        currentChunkCenter.x = currentPlayerChunkPosition.x + world.worldData.worldSettings.chunkSize.x / 2;
        currentChunkCenter.y = currentPlayerChunkPosition.y + world.worldData.worldSettings.chunkSize.y / 2;
        currentChunkCenter.z = currentPlayerChunkPosition.z + world.worldData.worldSettings.chunkSize.z / 2;
    }

    IEnumerator CheckIfShouldLoadNextPosition(){

        yield return new WaitForSeconds(detectionTime);

        if(
            Mathf.Abs(currentChunkCenter.x - player.transform.position.x) > world.worldData.worldSettings.chunkSize.x ||
            Mathf.Abs(currentChunkCenter.y - player.transform.position.y) > world.worldData.worldSettings.chunkSize.y ||
            Mathf.Abs(currentChunkCenter.z - player.transform.position.z) > world.worldData.worldSettings.chunkSize.z
        ){

            if(loadAdditionalChunks){
                world.LoadAdditionalChunksRequest(player);
            }
            
            
        }
        else{
            StartCoroutine(CheckIfShouldLoadNextPosition());
        }
    }

    

}
