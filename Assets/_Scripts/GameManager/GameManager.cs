using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.AI;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player;
    
    public GameObject RenderedChunks;
    public Vector3Int currentPlayerChunkPosition;
    public Vector3Int currentChunkCenter = Vector3Int.zero;

    public bool loadAdditionalChunks = false;

    public World world;

    public float detectionTime = 0.5f;
    public CinemachineVirtualCamera camera_VM;

    public UnityEvent inWater, onSolid;

    private NavMeshSurface surface;

    public void Awake(){

        if(RenderedChunks == null){

            Debug.Log("Rendered Chunks is null or dosen't have Navmesh Component, pls fix");

        }
        
        camera_VM.transform.position = new Vector3(0, world.worldSettings.voxelMaxMapDimensions.y * 4, 0);
        camera_VM.transform.rotation *= Quaternion.Euler(Vector3.right * 90);
    }

    public void Update(){

        if(player == null){
            return;
        }

        CheckIfInWater();

    }

    void CheckIfInWater()
    {

        VoxelType voxelType = WorldDataHelper.GetVoxelFromWorldCoorinates(world, Vector3Int.RoundToInt(player.transform.position));

        if(voxelType == VoxelType.Water){
            
            inWater?.Invoke();
            
        }
        else{
            
            onSolid?.Invoke();
        }

    }

    public void SpawnPlayer(){

        if (player != null)
            return;
        Vector3Int raycastStartposition = world.startingPosition;
        raycastStartposition.y = world.worldSettings.voxelMaxMapDimensions.y;
        RaycastHit hit;

        CreateNavMeshes();
        
        if (Physics.Raycast(raycastStartposition, Vector3.down, out hit, world.worldSettings.voxelMaxMapDimensions.y - world.worldSettings.voxelMinMapDimensions.y + 30))
        {

            player = Instantiate(playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
            camera_VM.Follow = player.transform.GetChild(0);

            inWater.AddListener(player.GetComponent<Character>().InWater);
            onSolid.AddListener(player.GetComponent<Character>().OnSolid);

            StartCheckingTheMap();
            
            return;
        }

        Debug.Log("Player not spawned");

        Vector3Int airSpawn = Vector3Int.zero;
        airSpawn.y = world.worldSettings.voxelMaxMapDimensions.y + 30;

        player = Instantiate(playerPrefab, airSpawn , Quaternion.identity);
        camera_VM.Follow = player.transform.GetChild(0);
        StartCheckingTheMap();
        
        
    }

    private void CreateNavMeshes()
    {
        surface = RenderedChunks.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        

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
