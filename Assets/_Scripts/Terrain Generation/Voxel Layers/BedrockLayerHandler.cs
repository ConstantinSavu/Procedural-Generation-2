using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedrockLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight, ref VoxelType currentVoxel){

        Vector3Int worldPos = pos + data.worldPosition;
        Vector3Int start = - Vector3Int.one + (data.worldReference.worldData.worldSettings.chunkDrawingRange + Vector3Int.one) * data.worldReference.worldData.worldSettings.chunkSize;
        Vector3Int end = - (data.worldReference.worldData.worldSettings.chunkDrawingRange) * data.worldReference.worldData.worldSettings.chunkSize;

        bool placeObsidian = false;

        if(worldPos.y >= surfaceHeight){
            return false;
        }

        if(worldPos.y  == end.y){
            placeObsidian = true;
        }


        if(!placeObsidian){
            return false;
        }

        currentVoxel = VoxelType.Bedrock;
        Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Bedrock);
        return true;

    }
}
