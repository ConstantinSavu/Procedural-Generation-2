using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveLayer : VoxelLayerHandler
{
    

    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight){

        Vector3Int worldPos = pos + data.worldPosition;

        if(worldPos.y >= surfaceHeight){
            return false;
        }

        float normalisedWorldHeight = CustomNoise.MapFloatValue(worldPos.y,
            data.worldReference.worldData.worldSettings.voxelMinMapDimensions.y,
            data.worldReference.worldData.worldSettings.voxelMaxMapDimensions.y,
            0f,
            1f
        );
        

        float noiseDensity = CustomNoise.OctaveNoise3D(worldPos, data.noiseSettings);
        float skewFactor = -Mathf.Pow(normalisedWorldHeight, data.noiseSettings.skewExponent) + 1;
        noiseDensity *= skewFactor;
        

        if(noiseDensity < data.noiseSettings.threshold){
            
            Chunk.SetVoxelFromChunkCoordinates(data, pos, VoxelType.Air);
        }

        return true;

    }


}
