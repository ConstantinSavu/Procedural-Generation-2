using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderGroundLayer : VoxelLayerHandler
{
    public VoxelType nearGroundVoxelType;
    public VoxelType underGroundVoxelType;
    public int nearGroundOffset = 5;

    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight){

        int worldPosY = pos.y + data.worldPosition.y;

        if(worldPosY >= surfaceHeight){
            return false;
        }

        VoxelType voxelType;
        if(worldPosY >= surfaceHeight - nearGroundOffset){
            voxelType = nearGroundVoxelType;
        }
        else{
            voxelType = underGroundVoxelType;
        }

        Chunk.SetVoxelFromChunkCoordinates(data, pos, voxelType);
        return true;

    }
}
