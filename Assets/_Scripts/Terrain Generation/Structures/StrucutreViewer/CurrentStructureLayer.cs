using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentStructureLayer : StructreLayerHandler
{

    public StructureSO currentStructure;
    protected override bool TryHandling(ChunkData data, Vector3Int treePos, BiomeGenerator biomeGenerator)
    {
        
        PlaceStructure(data, treePos);
 
        return true;
    }

    private void PlaceStructure(ChunkData data, Vector3Int localPos)
    {
        
        foreach (StructureVoxel structureVoxel in currentStructure.strucutreList)
        {
            Chunk.SetVoxelFromChunkCoordinates(data, localPos + structureVoxel.voxelPosition, structureVoxel.voxelType);
        }

        
    }
}

