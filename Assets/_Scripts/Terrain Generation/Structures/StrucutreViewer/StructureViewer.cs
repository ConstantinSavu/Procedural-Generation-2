using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureViewer : BiomeGenerator
{
    public override ChunkData ProcessVoxel(ChunkData data, Vector3Int pos, int groundPosition){
    

        
        Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Air);
        
        

        return data;

    }

    public StructurePositionsSO structurePositions;
    public override ChunkData ProcessStructures(ChunkData data){

        
        if(!structurePositions.worldStructurePositions.Contains(data.worldPosition)){
            return data;
        }


        List<Vector3Int> structureData = structurePositions.localStructurePositions;

        foreach(StructureGenerator structureGenerator in structureGenerators){
            

            foreach(Vector3Int localPos in structureData){
                structureGenerator.firstStructureLayerHandler.Handle(data, localPos, this);
            }
            
            

            foreach(var layer in structureGenerator.additionalStructureLayerHandlers){
                foreach(Vector3Int localPos in structureData){
                    structureGenerator.firstStructureLayerHandler.Handle(data, localPos, this);
                }
            }
        }

        return data;
    }

}
