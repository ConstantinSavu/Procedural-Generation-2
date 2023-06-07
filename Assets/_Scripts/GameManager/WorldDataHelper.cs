using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class WorldDataHelper
{
    
    public static Vector3Int ChunkPositionFromVoxelCoords(World world, Vector3Int pos){

        return new Vector3Int(
            Mathf.FloorToInt(pos.x / (float)world.worldData.worldSettings.chunkSize.x) * world.worldData.worldSettings.chunkSize.x,
            Mathf.FloorToInt(pos.y / (float)world.worldData.worldSettings.chunkSize.y) * world.worldData.worldSettings.chunkSize.y,
            Mathf.FloorToInt(pos.z / (float)world.worldData.worldSettings.chunkSize.z) * world.worldData.worldSettings.chunkSize.z
        );

    }

    public static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        Vector3Int start = playerPosition - (world.worldData.worldSettings.chunkDrawingRange) * world.worldData.worldSettings.chunkSize;
        Vector3Int end = playerPosition + (world.worldData.worldSettings.chunkDrawingRange) * world.worldData.worldSettings.chunkSize;
        

        List<Vector3Int> chunkPositionsToCreate = new List<Vector3Int>();
        for (int x = start.x; x <= end.x; x += world.worldData.worldSettings.chunkSize.x)
        {
            for (int z = start.z; z <= end.z; z += world.worldData.worldSettings.chunkSize.z)
            {
                for (int y = start.y; y <= end.y; y += world.worldData.worldSettings.chunkSize.y)
                {
                    Vector3Int chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, y, z));
                    chunkPositionsToCreate.Add(chunkPos);
                }
            }
        }

        return chunkPositionsToCreate;
    }

    public static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPosition){
        Vector3Int start = playerPosition - (world.worldData.worldSettings.chunkDrawingRange + Vector3Int.one) * world.worldData.worldSettings.chunkSize;
        Vector3Int end = playerPosition + (world.worldData.worldSettings.chunkDrawingRange + Vector3Int.one) * world.worldData.worldSettings.chunkSize;
        

        List<Vector3Int> dataPositionsToCreate = new List<Vector3Int>();
        for (int x = start.x; x <= end.x; x += world.worldData.worldSettings.chunkSize.x)
        {
            for (int z = start.z; z <= end.z; z += world.worldData.worldSettings.chunkSize.z)
            {
                for (int y = start.y; y <= end.y; y += world.worldData.worldSettings.chunkSize.y)
                {
                    Vector3Int chunkPos = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, y, z));
                    dataPositionsToCreate.Add(chunkPos);
                }
            }
        }

        return dataPositionsToCreate;
    }

    public static List<Vector3Int> SelectPositonsToCreate(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPosition){
        return allChunkPositionsNeeded
                .Where(pos => worldData.chunkDictionary.ContainsKey(pos) == false)
                .OrderBy(pos => Vector3.Distance(playerPosition, pos))
                .ToList();
                
    }

    public static List<Vector3Int> SelectDataPositonsToCreate(World.WorldData worldData, List<Vector3Int> allDataPositionsNeeded, Vector3Int playerPosition){
        return allDataPositionsNeeded
                .Where(pos => worldData.chunkDataDictionary.ContainsKey(pos) == false)
                .OrderBy(pos => Vector3.Distance(playerPosition, pos))
                .ToList();
    }

    public static List<Vector3Int> GetUnnededData(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded){
        return worldData.chunkDataDictionary.Keys
                .Where(pos => allChunkPositionsNeeded.Contains(pos) == false)
                .ToList();
    }

    public static List<Vector3Int> GetUnnededChunks(World.WorldData worldData, List<Vector3Int> allChunkPositionsNeeded)
    {
        return worldData.chunkDictionary.Keys
                .Where(pos => allChunkPositionsNeeded.Contains(pos) == false)
                .ToList();
    }

    public static void RemoveChunk(World world, Vector3Int pos){

        ChunkRenderer chunk = null;

        if(world.worldData.chunkDictionary.TryGetValue(pos, out chunk)){
            world.worldRenderer.RemoveChunk(chunk);
            world.worldData.chunkDictionary.TryRemove(pos, out _);
            return;
        }

        Debug.Log("Could not remove chunk");


    }

    public static void RemoveChunkData(World world, Vector3Int pos){
        world.worldData.chunkDataDictionary.TryRemove(pos, out _);
    }

    public static bool SetVoxelFromWorldCoordinates(World world, Vector3Int worldPos, VoxelType voxelType){

        ChunkData chunkData = GetChunkDataFromWorldCoordinates(world, worldPos);

        if(chunkData == null){
            return false;
        }

        Vector3Int localPosition = Chunk.GetVoxelInChunkCoordinates(chunkData, worldPos);
        
        return Chunk.SetVoxelFromChunkCoordinates(chunkData, localPosition, voxelType);

    }

    public static bool SetVoxelFromWorldCoordinates(World world, Vector3Int worldPos, VoxelType voxelType, out ChunkData chunkData){

        chunkData = GetChunkDataFromWorldCoordinates(world, worldPos);

        if(chunkData == null){
            return false;
        }

        Vector3Int localPosition = Chunk.GetVoxelInChunkCoordinates(chunkData, worldPos);
        
        return Chunk.SetVoxelFromChunkCoordinates(chunkData, localPosition, voxelType);

    }

    public static VoxelType GetVoxelFromWorldCoorinates(World world, Vector3Int worldPos){

        ChunkData chunkData = GetChunkDataFromWorldCoordinates(world, worldPos);

        if(chunkData == null){
            return VoxelType.Nothing;
        }

        Vector3Int localPosition = Chunk.GetVoxelInChunkCoordinates(chunkData, worldPos);
        return Chunk.GetVoxelFromChunkCoordinates(chunkData, localPosition);

    }

    public static ChunkData GetChunkDataFromWorldCoordinates(World world, Vector3Int worldPos){

        Vector3Int chunkPosition = ChunkPositionFromVoxelCoords(world, worldPos);

        ChunkData containerChunk = null;

        world.worldData.chunkDataDictionary.TryGetValue(chunkPosition, out containerChunk);

        return containerChunk;

    }

    public static ChunkRenderer GetChunk(World world, Vector3Int pos){

        if(world.worldData.chunkDictionary.ContainsKey(pos)){
            return world.worldData.chunkDictionary[pos];
        }

        return null;

    }

}
