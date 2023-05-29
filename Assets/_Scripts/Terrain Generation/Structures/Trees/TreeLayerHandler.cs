using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLayerHandler : StructreLayerHandler
{

    public float terrainHeightLimit = 40;
    public StructureSO treeStrucutre;
    protected override bool TryHandling(ChunkData data, Vector3Int treePos, BiomeGenerator biomeGenerator)
    {

        int wolrdGroundPosition = biomeGenerator.Get2DTerrainY(treePos.x, treePos.z, data);
        int localGroundPosition = wolrdGroundPosition - data.worldPosition.y;
        
        treePos.y = localGroundPosition;

        if(treePos.x == 0 || treePos.y == 0 || treePos.z == 0){
            return false;
        }

        if( treePos.x == data.worldReference.worldSettings.chunkSize.x ||
            treePos.y == data.worldReference.worldSettings.chunkSize.y ||
            treePos.z == data.worldReference.worldSettings.chunkSize.z){
            return false;
        }
        
        //get the type of the block underneath
        VoxelType type = Chunk.GetVoxelFromChunkCoordinates(data, treePos);

        if(type != VoxelType.Grass_Dirt){
            return false;
        }
        
        PlaceTree(data, treePos);

        return true;
    }

    private void PlaceTree(ChunkData data, Vector3Int localPos)
    {
        
        foreach (StructureVoxel structureVoxel in treeStrucutre.strucutreList)
        {
            Chunk.SetVoxelFromChunkCoordinates(data, localPos + structureVoxel.voxelPosition, structureVoxel.voxelType);
        }

        
    }
}

