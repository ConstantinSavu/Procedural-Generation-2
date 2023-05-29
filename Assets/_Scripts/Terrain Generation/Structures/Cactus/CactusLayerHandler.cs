using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusLayerHandler : StructreLayerHandler
{

    public float terrainHeightLimit = 40;
    public StructureSO cactusStructure;

    public int maxAdditionalCactusHeight = 3;
    public int pseudoRandomOffset = 434245;
    protected override bool TryHandling(ChunkData data, Vector3Int treePos, BiomeGenerator biomeGenerator)
    {

        int worldGroundPosition = biomeGenerator.Get2DTerrainY(treePos.x, treePos.z, data);
        int localGroundPosition = worldGroundPosition - data.worldPosition.y;
        
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

        if(type != VoxelType.Sand){
            return false;
        }
        
        PlaceCactus(data, treePos, worldGroundPosition);

        return true;
    }

    private void PlaceCactus(ChunkData data, Vector3Int localPos, int worldGroundPosition)
    {
        Vector3Int lastPos = Vector3Int.zero;
        foreach (StructureVoxel structureVoxel in cactusStructure.strucutreList)
        {
            lastPos = localPos + structureVoxel.voxelPosition;
            Chunk.SetVoxelFromChunkCoordinates(data, lastPos, structureVoxel.voxelType);
        }

        int additionalBlocks = (worldGroundPosition + pseudoRandomOffset) % maxAdditionalCactusHeight;

        for(int i = 0; i < additionalBlocks; i++){
            lastPos.y += 1;
            Chunk.SetVoxelFromChunkCoordinates(data, lastPos, VoxelType.Cactus);
        }
        
    }
}

