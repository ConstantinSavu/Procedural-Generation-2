using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLayerHandler : VoxelLayerHandler
{
    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight){

        int worldPosY = pos.y + data.worldPosition.y;

        if(worldPosY <= surfaceHeight){
            return false;
        }

        VoxelType voxelType;
        voxelType = VoxelType.Air;

        Chunk.SetVoxelFromChunkCoordinates(data, pos, voxelType);
        return true;
    }
}
