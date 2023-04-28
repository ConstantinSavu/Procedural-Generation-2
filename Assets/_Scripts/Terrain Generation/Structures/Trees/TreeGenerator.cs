using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TreeGenerator : StructureGenerator
{

    public NoiseSettings structureNoiseSettings;
    public DomainWarping domainWarping;
    public bool useDomainWarping = false;
    
    public override List<Vector3Int> GenerateStructureData(ChunkData chunkData){

        structureNoiseSettings.worldOffset = chunkData.worldReference.worldData.worldSettings.mapSeedOffset;

        float[,] noiseData = GenerateStructureNoise(chunkData, structureNoiseSettings);
        
        return DataProcessing.FindLocalMaxima2D(noiseData, chunkData.worldPosition);

    }

    private float[,] GenerateStructureNoise(ChunkData chunkData, NoiseSettings treeNoiseSettings)
    {
        float[,] noiseMax = new float[chunkData.chunkSize.x, chunkData.chunkSize.z];

        Vector3Int stop = chunkData.worldPosition + chunkData.chunkSize;
        Vector3Int start = chunkData.worldPosition;

        Func<Vector2Int, NoiseSettings, float> noiseFunction = useDomainWarping ? 
            domainWarping.GenerateDomainNoise2D: 
            CustomNoise.OctavePerlin2D;

        int xIndex = 0;
        int zIndex = 0;

        for(int x = start.x; x < stop.x; x++){

            zIndex = 0;

            for(int z = start.z; z < stop.z; z++){
                
                noiseMax[xIndex, zIndex] = noiseFunction(new Vector2Int(x, z), treeNoiseSettings);
                
                zIndex++;
            }

            xIndex++;
        }


        return noiseMax;
    }
}
