using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight){

        VoxelType voxelType; 

        int worldPosY = pos.y + data.worldPosition.y;

        if(worldPosY < surfaceHeight) {
            return false;
        }
        
        if(worldPosY > data.worldReference.worldData.worldSettings.waterThreshold){
            return false;
        }
        
        voxelType = VoxelType.Water;

        if(worldPosY == surfaceHeight){

            voxelType = VoxelType.Sand;

        }

        if(worldPosY == surfaceHeight + 1){

            voxelType = VoxelType.Sand;

        }

        if(worldPosY == surfaceHeight + 2){

            voxelType = VoxelType.Sand;

        }

        Chunk.SetVoxelFromChunkCoordinates(data, pos, voxelType);
        return true;

        

    }
}
