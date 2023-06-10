using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralLayerHandler : StructreLayerHandler
{

    public float waterSurfaceLimit = 3;
    public StructureSO coralStrucutre;

    StructureSO copyCurrentStructure;

    public int pseudoRandomOffset = 626622;

    public void Awake(){
        copyCurrentStructure = Instantiate(coralStrucutre);
    }
    protected override bool TryHandling(ChunkData data, Vector3Int localPos, BiomeGenerator biomeGenerator)
    {

        int worldGroundPosition = biomeGenerator.Get2DTerrainY(localPos.x, localPos.z, data);
        int localGroundPosition = worldGroundPosition - data.worldPosition.y;
        localPos.y = localGroundPosition + 2;

        Vector3Int worldPos = localPos + data.worldPosition;
        
        if(worldPos.y > data.worldReference.worldSettings.waterThreshold - waterSurfaceLimit){
            return false;
        }

        if(localPos.x == 0 || localPos.y == 0 || localPos.z == 0){
            return false;
        }

        if( localPos.x == data.worldReference.worldSettings.chunkSize.x ||
            localPos.y == data.worldReference.worldSettings.chunkSize.y ||
            localPos.z == data.worldReference.worldSettings.chunkSize.z){
            return false;
        }
        
        //get the type of the block underneath
        VoxelType type = Chunk.GetVoxelFromChunkCoordinates(data, localPos);

        if(type != VoxelType.Sand){
            return false;
        }

        RotateStructure(worldGroundPosition);
        PlaceCoral(data, localPos, worldPos);

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

    private void PlaceCoral(ChunkData data, Vector3Int localPos, Vector3Int worldPos)
    {
        
        foreach (StructureVoxel structureVoxel in copyCurrentStructure.strucutreList)
        {   
            if(worldPos.y + structureVoxel.voxelPosition.y > 
                data.worldReference.worldSettings.waterThreshold - 1){
                    continue;
            }

            Chunk.SetVoxelFromChunkCoordinates(data, localPos + structureVoxel.voxelPosition, structureVoxel.voxelType);
        }

        
    }
}

