using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLayerHandler : StructreLayerHandler
{

    public float terrainHeightLimit = 40;
    public StructureSO treeStrucutre;

    StructureSO copyCurrentStructure;

    public int pseudoRandomOffset = 626622;

    public void Awake(){
        copyCurrentStructure = Instantiate(treeStrucutre);
    }
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
        RotateStructure(wolrdGroundPosition);
        PlaceTree(data, treePos);

        return true;
    }

    public List<Vector3Int> angleChangeList = new List<Vector3Int>{
        new Vector3Int(0, 90, 0),
        new Vector3Int(0, 180, 0),
        new Vector3Int(0, 270, 0),
        new Vector3Int(0, 360, 0)
    };
    private void RotateStructure(int worldGroundPosition)
    {

        int angleIndex = (worldGroundPosition + pseudoRandomOffset) % angleChangeList.Count;

        Vector3Int angleChange = angleChangeList[angleIndex];

        foreach (StructureVoxel structureVoxel in copyCurrentStructure.strucutreList)
        {
            Vector3 rotatedVector = Quaternion.AngleAxis(angleChange.y, Vector3.up) * structureVoxel.voxelPosition;
            rotatedVector = Quaternion.AngleAxis(angleChange.z, Vector3.forward) * rotatedVector;
            rotatedVector = Quaternion.AngleAxis(angleChange.z, Vector3.right) * rotatedVector;
            
            StructureVoxel newStrucutrVoxel = new StructureVoxel();
            
            Vector3Int rotatedVectorInt = Vector3Int.RoundToInt(rotatedVector);
            
            structureVoxel.voxelPosition = rotatedVectorInt;
        }

    }

    private void PlaceTree(ChunkData data, Vector3Int localPos)
    {
        
        foreach (StructureVoxel structureVoxel in copyCurrentStructure.strucutreList)
        {
            Chunk.SetVoxelFromChunkCoordinates(data, localPos + structureVoxel.voxelPosition, structureVoxel.voxelType);
        }

        
    }
}

