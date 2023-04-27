using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player;
    
    public Vector3Int currentPlayerChunkPosition;
    public Vector3Int currentChunkCenter = Vector3Int.zero;

    public World world;

    public float detectionTime = 0.5f;
    public CinemachineVirtualCamera camera_VM;

    public UnityEvent inWater, onSolid; 

    public void Awake(){
        
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
        Debug.Log(world.worldSettings.voxelMaxMapDimensions.y);
        Debug.Log(world.worldSettings.voxelMinMapDimensions.y);
        Debug.Log(world.worldSettings.voxelMaxMapDimensions.y - world.worldSettings.voxelMinMapDimensions.y + 30);
        player = Instantiate(playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
        camera_VM.Follow = player.transform.GetChild(0);
        StartCheckingTheMap();
        
        
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
            
            //world.LoadAdditionalChunksRequest(player);
            
        }
        else{
            StartCoroutine(CheckIfShouldLoadNextPosition());
        }
    }

    

}
