using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldDataHelper
{
    public static Vector3Int GetPositionFromIndex(WorldSettings worldSettings, int index){

        int x = index % worldSettings.chunkDrawingRange.x;
        int y = (index / worldSettings.chunkDrawingRange.x) % worldSettings.chunkDrawingRange.y;
        int z = index / (worldSettings.chunkDrawingRange.x * worldSettings.chunkDrawingRange.y);


        return new Vector3Int(x, y, z);
    }

    public static Vector3Int GetMatrixPositionFromWorldPosition(WorldSettings worldSettings, Vector3Int chunkRange, Vector3Int worldPosition){
        
        Vector3Int matrixChunkPosition = new Vector3Int(
            worldPosition.x / worldSettings.chunkSize.x,
            worldPosition.y / worldSettings.chunkSize.y,
            worldPosition.z / worldSettings.chunkSize.z
        );
        
        return new Vector3Int(
            matrixChunkPosition.x % chunkRange.x + chunkRange.x,
            matrixChunkPosition.y % chunkRange.y + chunkRange.y,
            matrixChunkPosition.z % chunkRange.z + chunkRange.z
        );
    }

    public static int GetIndexFromPosition(WorldSettings worldSettings, Vector3Int worldPosition){

        return worldPosition.x + worldSettings.chunkDrawingRange.x * worldPosition.y + worldSettings.chunkDrawingRange.x * worldSettings.chunkDrawingRange.y * worldPosition.z;

    }
    public static Vector3Int ChunkPositionFromVoxelCoords(World world, Vector3Int pos){

        return new Vector3Int(
            Mathf.FloorToInt(pos.x / (float)world.worldData.worldSettings.chunkSize.x) * world.worldData.worldSettings.chunkSize.x,
            Mathf.FloorToInt(pos.y / (float)world.worldData.worldSettings.chunkSize.y) * world.worldData.worldSettings.chunkSize.y,
            Mathf.FloorToInt(pos.z / (float)world.worldData.worldSettings.chunkSize.z) * world.worldData.worldSettings.chunkSize.z
        );

    }

    public static List<Vector3Int> GetChunkRendererPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        Vector3Int start = playerPosition - (world.worldData.worldSettings.chunkDrawingRange) * world.worldData.worldSettings.chunkSize;
        Vector3Int end = playerPosition + (world.worldData.worldSettings.chunkDrawingRange) * world.worldData.worldSettings.chunkSize;
        

        WorldSettings worldSettings = world.worldSettings;
        Vector3Int chunkDrawingRange = world.worldSettings.chunkDrawingRange;

        List<Vector3Int> chunkRendererPositionsToCreate = new List<Vector3Int>();
        for (int x = start.x; x <= end.x; x += world.worldData.worldSettings.chunkSize.x)
        {
            for (int z = start.z; z <= end.z; z += world.worldData.worldSettings.chunkSize.z)
            {
                for (int y = start.y; y <= end.y; y += world.worldData.worldSettings.chunkSize.y)
                {
                    Vector3Int chunkWorldPosition = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, y, z));

                    Vector3Int matrixPosition = GetMatrixPositionFromWorldPosition(world.worldSettings, chunkDrawingRange, chunkWorldPosition);

                    int index = GetIndexFromPosition(world.worldSettings, matrixPosition);

                    ChunkRenderer currentChunk = world.worldData.chunkRendererMatrix[index];

                    if(currentChunk == null){
                        chunkRendererPositionsToCreate.Add(chunkWorldPosition);
                        continue;
                    }


                    if(currentChunk.ChunkData.worldPosition != chunkWorldPosition){
                        chunkRendererPositionsToCreate.Add(chunkWorldPosition);
                    }
                }
            }
        }

        return chunkRendererPositionsToCreate;
    }

    public static List<Vector3Int> GetChunkDataPositionsAroundPlayer(World world, Vector3Int playerPosition){
        Vector3Int start = playerPosition - (world.worldData.worldSettings.chunkDrawingRange + Vector3Int.one) * world.worldData.worldSettings.chunkSize;
        Vector3Int end = playerPosition + (world.worldData.worldSettings.chunkDrawingRange + Vector3Int.one) * world.worldData.worldSettings.chunkSize;
        
        WorldSettings worldSettings = world.worldSettings;
        Vector3Int chunkDrawingRange = world.worldSettings.chunkDrawingRange;

        List<Vector3Int> dataPositionsToCreate = new List<Vector3Int>();
        for (int x = start.x; x <= end.x; x += world.worldData.worldSettings.chunkSize.x)
        {
            for (int z = start.z; z <= end.z; z += world.worldData.worldSettings.chunkSize.z)
            {
                for (int y = start.y; y <= end.y; y += world.worldData.worldSettings.chunkSize.y)
                {
                    Vector3Int chunkWorldPosition = ChunkPositionFromVoxelCoords(world, new Vector3Int(x, y, z));

                    Vector3Int matrixPosition = GetMatrixPositionFromWorldPosition(world.worldSettings, chunkDrawingRange + Vector3Int.one, chunkWorldPosition);

                    int index = GetIndexFromPosition(world.worldSettings, matrixPosition);

                    ChunkRenderer currentChunk = world.worldData.chunkRendererMatrix[index];

                    if(currentChunk == null){
                        dataPositionsToCreate.Add(chunkWorldPosition);
                        continue;
                    }

                    if(currentChunk.ChunkData.worldPosition != chunkWorldPosition){
                        dataPositionsToCreate.Add(chunkWorldPosition);
                    }
                }
            }
        }

        return dataPositionsToCreate;
    }

    public static bool SetVoxelFromWorldCoordinates(World world, Vector3Int worldPos, VoxelType voxelType){

        ChunkData chunkData = GetChunkDataFromWorldCoordinates(world, worldPos);

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

        Vector3Int chunkWorldPosition = ChunkPositionFromVoxelCoords(world, worldPos);

        WorldSettings worldSettings = world.worldSettings;
        Vector3Int chunkDrawingRange = world.worldSettings.chunkDrawingRange;

        Vector3Int matrixPosition = GetMatrixPositionFromWorldPosition(world.worldSettings, chunkDrawingRange + Vector3Int.one, chunkWorldPosition);

        int index = GetIndexFromPosition(worldSettings, matrixPosition);

        return world.worldData.chunkDataMatrix[index];

    }

    public static ChunkRenderer GetChunkRendererFromWorldPosition(World world, Vector3Int worldPos){

        Vector3Int chunkWorldPosition = ChunkPositionFromVoxelCoords(world, worldPos);

        WorldSettings worldSettings = world.worldSettings;
        Vector3Int chunkDrawingRange = world.worldSettings.chunkDrawingRange;

        Vector3Int matrixPosition = GetMatrixPositionFromWorldPosition(world.worldSettings, chunkDrawingRange, chunkWorldPosition);

        int index = GetIndexFromPosition(worldSettings, matrixPosition);

        return world.worldData.chunkRendererMatrix[index];

    }

    public static void AddChunkDataToChunkDataMatrix(World world, Vector3Int worldPosition, ChunkData chunkData)
    {
        WorldSettings worldSettings = world.worldSettings;
        Vector3Int chunkDrawingRange = world.worldSettings.chunkDrawingRange;

        Vector3Int matrixPosition = GetMatrixPositionFromWorldPosition(world.worldSettings, chunkDrawingRange + Vector3Int.one, worldPosition);

        int index = GetIndexFromPosition(worldSettings, matrixPosition);

        world.worldData.chunkDataMatrix[index] = chunkData;
    }

    public static void AddChunkRendererToChunkDataMatrix(World world, Vector3Int worldPosition, ChunkData chunkData)
    {
        WorldSettings worldSettings = world.worldSettings;
        Vector3Int chunkDrawingRange = world.worldSettings.chunkDrawingRange;

        Vector3Int matrixPosition = GetMatrixPositionFromWorldPosition(world.worldSettings, chunkDrawingRange + Vector3Int.one, worldPosition);

        int index = GetIndexFromPosition(worldSettings, matrixPosition);

        world.worldData.chunkDataMatrix[index] = chunkData;
    }
}
}
