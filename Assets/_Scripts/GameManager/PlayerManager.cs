using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
    
    [SerializeField] private Vector3Int currentPlayerChunkPosition;
    [SerializeField] private Vector3Int currentChunkCenter = Vector3Int.zero;
    [SerializeField] float detectionTime = 0.5f;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform player;

    World world;
    CinemachineVirtualCamera camera_VM;

    public bool loadAdditionalChunks = false;

    public void SetupPlayerManager(World world, CinemachineVirtualCamera camera_VM){
        this.world = world;
        this.camera_VM = camera_VM;
        SpawnPlayer();
    }

    public void SpawnPlayer(){

        if (player != null)
            return;

        Vector3 raycastStartposition = world.worldSettings.startingPosition;
        raycastStartposition.y = world.worldSettings.maxMapDimensions.y;

        float rayCastLength = world.worldSettings.maxMapDimensions.y - world.worldSettings.minMapDimensions.y + 30;

        RaycastHit hit;
        
        if (Physics.Raycast(raycastStartposition, Vector3.down, out hit, rayCastLength))
        {
            
            SpawnPlayer(hit);
            StartCheckingTheMap();
            
            return;
        }

        Debug.Log("Player not spawned");

        SpawnPlayer(world.worldSettings.maxMapDimensions);
        StartCheckingTheMap();
        
        
    }

    private void SpawnPlayer(RaycastHit hit){
        GameObject playerGameObject;
        playerGameObject = Instantiate(playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
        player = playerGameObject.transform;
        PlayerCamera playerCamera = player.GetComponentInChildren<PlayerCamera>();
        playerCamera.SetCamera(camera_VM);
    }

    private void SpawnPlayer(Vector3 spawnPosition){
        GameObject playerGameObject;
        playerGameObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        player = playerGameObject.transform;
        PlayerCamera playerCamera = player.GetComponentInChildren<PlayerCamera>();
        playerCamera.SetCamera(camera_VM);
    }

    public Transform GetPlayer(){
        return player;
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
                world.LoadAdditionalChunksRequest(player.position);
            }
            
            
        }
        else{
            StartCoroutine(CheckIfShouldLoadNextPosition());
        }
    }

    

}
