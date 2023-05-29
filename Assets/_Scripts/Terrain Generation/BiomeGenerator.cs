using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{

    
    
    
    public NoiseSettings biomeNoiseSettings;

    public DomainWarping domainWarping;
    public bool useDomainWarping = true;

    public VoxelLayerHandler firstLayer;

    public List<VoxelLayerHandler> additionalLayerHandlres;

    public List<StructureGenerator> structureGenerators;

    public virtual ChunkData ProcessVoxel(ChunkData data, Vector3Int pos, int groundPosition){

        biomeNoiseSettings.worldOffset = data.worldReference.worldData.worldSettings.mapSeedOffset;
        data.noiseSettings = biomeNoiseSettings;
        
        firstLayer.Handle(data, pos, groundPosition);

        foreach(var layer in additionalLayerHandlres){
            layer.Handle(data, pos, groundPosition);
        }

        return data;

    }

    public virtual ChunkData ProcessStructures(ChunkData data)
    {
        
        foreach(StructureGenerator structureGenerator in structureGenerators){
            List<Vector3Int> structureData = structureGenerator.GenerateStructureData(data);

            foreach(Vector3Int localPos in structureData){
                structureGenerator.firstStructureLayerHandler.Handle(data, localPos, this);
            }
            
            

            foreach(var layer in structureGenerator.additionalStructureLayerHandlers){
                foreach(Vector3Int localPos in structureData){
                    layer.Handle(data, localPos, this);
                }
            }
        }

        return data;
    }

    public int Get2DTerrainY(int x, int z, ChunkData data){

        int heightMapIndex = x * data.chunkSize.x + z;

        if(data.heightMap[heightMapIndex] != 0){
            return data.heightMap[heightMapIndex];
        }

        int worldPosX = x + data.worldPosition.x;
        int worldPosZ = z + data.worldPosition.z;

        float terrainNoise;
        if(!useDomainWarping){

            terrainNoise = CustomNoise.OctaveNoise2D(new Vector2Int(worldPosX,worldPosZ), biomeNoiseSettings);

        }
        else{

            terrainNoise = domainWarping.GenerateDomainNoise2D(new Vector2Int(worldPosX,worldPosZ), biomeNoiseSettings);

        }

        

        //Redistribute terrain
        //float terrain = CustomNoise.ResistributeNoise(terrainNoise, biomeNoiseSettings);

        biomeNoiseSettings.minDimension.y = data.worldReference.worldSettings.voxelMinMapDimensions.y;
        biomeNoiseSettings.maxDimension.y = data.worldReference.worldSettings.voxelMaxMapDimensions.y;
        
        int final_terrain_value = CustomNoise.MapNormalizedValueToInt(terrainNoise, biomeNoiseSettings.minDimension.y, biomeNoiseSettings.maxDimension.y);

        return final_terrain_value;

        /*
        biomeNoiseSettings.minDimension.y = Mathf.Clamp(
            biomeNoiseSettings.minDimension.y,
            0,
            1
        );

        biomeNoiseSettings.maxDimension.y = Mathf.Clamp(
            biomeNoiseSettings.maxDimension.y,
            biomeNoiseSettings.minDimension.y,
            1
        );

        float minDimension = CustomNoise.MapNormalizedValue(
            biomeNoiseSettings.minDimension.y,
            (float)data.worldReference.worldData.worldSettings.VoxelMinMapDimensions.y, 
            (float)data.worldReference.worldData.worldSettings.VoxelMaxMapDimensions.y
        );

        float maxDimension = CustomNoise.MapNormalizedValue(
            biomeNoiseSettings.maxDimension.y,
            (float)data.worldReference.worldData.worldSettings.VoxelMinMapDimensions.y, 
            (float)data.worldReference.worldData.worldSettings.VoxelMaxMapDimensions.y
        );

        int intMinDimensions = Mathf.CeilToInt(minDimension);
        int intMaxDimensions = Mathf.FloorToInt(maxDimension);



        //Map terrain value to minimum and maximum values for biome
        int final_terrain_value = CustomNoise.MapNormalizedValueToInt(terrain, intMinDimensions, intMaxDimensions);

        final_terrain_value = Mathf.Clamp(
            final_terrain_value + biomeNoiseSettings.offset.y,
            intMinDimensions,
            intMaxDimensions-1
        );

        return final_terrain_value;
        */

    }

    
}
