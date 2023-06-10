using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentStructureLayer : StructreLayerHandler
{

    public StructureSO currentStructure;
    StructureSO copyCurrentStructure;

    public void Awake(){
        copyCurrentStructure = Instantiate(currentStructure);
    }

    public void ReloadStrucutre(){
        Destroy(copyCurrentStructure);
        copyCurrentStructure = Instantiate(currentStructure);
    }

    public Vector3 angleChange = new Vector3(90f, 90f, 90f);
    protected override bool TryHandling(ChunkData data, Vector3Int treePos, BiomeGenerator biomeGenerator)
    {

        RotateStructure(angleChange);
        PlaceStructure(data, treePos);
 
        return true;
    }

    private void RotateStructure(Vector3 angleChange)
    {

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

    private void PlaceStructure(ChunkData data, Vector3Int localPos)
    {   

        
        foreach (StructureVoxel structureVoxel in copyCurrentStructure.strucutreList)
        {
            Chunk.SetVoxelFromChunkCoordinates(data, localPos + structureVoxel.voxelPosition, structureVoxel.voxelType);
        }

        
    }
}

