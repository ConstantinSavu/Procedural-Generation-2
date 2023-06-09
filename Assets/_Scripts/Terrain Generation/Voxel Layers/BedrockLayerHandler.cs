using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedrockLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight){

        Vector3Int worldPos = pos + data.worldPosition;
        Vector3Int start = - Vector3Int.one + (data.worldReference.worldData.worldSettings.chunkDrawingRange + Vector3Int.one) * data.worldReference.worldData.worldSettings.chunkSize;
        Vector3Int end = - (data.worldReference.worldData.worldSettings.chunkDrawingRange) * data.worldReference.worldData.worldSettings.chunkSize;

        if(worldPos.y  == end.y){
            Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Bedrock);
            return true;
        }
        /*
        if(worldPos.z  == end.z){
            Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Bedrock);
            return true;
        }

        if(worldPos.x  == end.x){
            Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Bedrock);
            return true;
        }

        if(worldPos.z  == start.z){
            Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Bedrock);
            return true;
        }

        if(worldPos.x  == start.x){
            Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Bedrock);
            return true;
        }
        */
        
        return false;

    }
}
