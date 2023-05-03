using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : MonoBehaviour
{   
    public NoiseSettings biomeNoiseSettings;

    public DomainWarping domainWarping;
    public bool useDomainWarping = true;

    public VoxelLayerHandler firstLayer;

    public List<VoxelLayerHandler> additionalLayerHandlres;

    public ChunkData ProcessChunk(ChunkData data, Vector3Int pos){

        biomeNoiseSettings.worldOffset = data.worldReference.worldData.worldSettings.mapSeedOffset;

        
        Vector3Int worldPos = new Vector3Int(pos.x + data.worldPosition.x, pos.y + data.worldPosition.y, pos.z + data.worldPosition.z);
        float noiseDensity;
        
        if(!useDomainWarping){

            noiseDensity = CustomNoise.OctaveNoise3D(worldPos, biomeNoiseSettings);

        }
        else{

            noiseDensity = domainWarping.GenerateDomainNoise3D(worldPos, biomeNoiseSettings);

        }

        if(biomeNoiseSettings.useSkew){

            float normalisedWorldHeight = CustomNoise.MapFloatValue(worldPos.y,
                data.worldReference.worldData.worldSettings.VoxelMinMapDimensions.y,
                data.worldReference.worldData.worldSettings.VoxelMaxMapDimensions.y,
                0f,
                1f
            );

            float skewFactor = -Mathf.Pow(normalisedWorldHeight, biomeNoiseSettings.skewExponent) + 1;

            noiseDensity *= skewFactor;
        }


        if(noiseDensity > biomeNoiseSettings.threshold){
            Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Stone);
        }
        else{
            Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Air);
        }

        return data;

    }

    private float SkewNoiseY(Vector3Int worldPos, float noiseDensity){


        return 0;
    }



    
    
    private int Get2DTerrainY(int x, int z, ChunkData data){

        
        //Get the noise for index noise_1, noise_2, returns normalized value
        float terrainNoise;
        if(!useDomainWarping){

            terrainNoise = CustomNoise.OctaveNoise2D(new Vector2Int(x,z), biomeNoiseSettings);

        }
        else{

            terrainNoise = domainWarping.GenerateDomainNoise2D(new Vector2Int(x,z), biomeNoiseSettings);


        }

        

        //Redistribute terrain
        float terrain = CustomNoise.ResistributeNoise(terrainNoise, biomeNoiseSettings);

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

    }

}
