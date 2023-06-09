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
        
        VoxelType currentVoxel = Chunk.GetVoxelFromChunkCoordinates(data, pos);

        firstLayer.Handle(data, pos, groundPosition, ref currentVoxel);

        foreach(var layer in additionalLayerHandlres){
            layer.Handle(data, pos, groundPosition, ref currentVoxel);
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

        int heightMapIndex = x * data.chunkSize.z + z;

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

    }

    
}
