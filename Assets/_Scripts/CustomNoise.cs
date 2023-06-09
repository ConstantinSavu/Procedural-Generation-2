using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CustomNoise
{

    static bool useSimplex = true;
    public static float Noise3D(float x, float y, float z){
        
        float noiseResult;
        
        if(useSimplex){
            noiseResult = FastNoiseLiteStatic.GetNoise(x, y, z);
            noiseResult = (noiseResult + 1.0f) / 2.0f;
        }
        else{
            float xy = Mathf.PerlinNoise(x, y);
            float xz = Mathf.PerlinNoise(x, z);
            float yz = Mathf.PerlinNoise(y, z);
            float yx = Mathf.PerlinNoise(y, x);
            float zx = Mathf.PerlinNoise(z, x);
            float zy = Mathf.PerlinNoise(z, y);
    
            noiseResult = (xy + xz + yz + yx + zx + zy) / 6.0f;
        }

        if(noiseResult < 0f){
            Debug.Log("Unexpected noise value " + noiseResult);
        }

        if(noiseResult > 1f){
            Debug.Log("Unexpected noise value " + noiseResult);
        }
        
    
        return noiseResult;
    }

    public static float Noise2D(float x, float z){

        float noiseResult;
        
        if(useSimplex){
            noiseResult = FastNoiseLiteStatic.GetNoise(x, z);
            noiseResult = (noiseResult + 1.0f) / 2.0f;
        }
        else{
            noiseResult = Mathf.PerlinNoise(x, z);
        }
        
        if(noiseResult < 0f){
            Debug.Log("Unexpected noise value " + noiseResult);
        }

        if(noiseResult > 1f){
            Debug.Log("Unexpected noise value " + noiseResult);
        }
        
        

        return noiseResult;
    }

    public static float OctaveNoise3D(Vector3Int pos, NoiseSettings settings){

        float x = (float)pos.x * settings.noiseZoom.x;
        float y = (float)pos.y * settings.noiseZoom.y;
        float z = (float)pos.z * settings.noiseZoom.z;

        x += settings.noiseZoom.x;
        y += settings.noiseZoom.y;
        z += settings.noiseZoom.z;

        float result = 0;
        float frequency = 1;
        float amplitude = 1;

        float totalAmplitude = 0;

        for(int i = 0; i < settings.octaves; i++){

            result += amplitude * Noise3D(
                (x + settings.localOffset.x + settings.worldOffset.x) * frequency,
                (y + settings.localOffset.y + settings.worldOffset.y) * frequency, 
                (z + settings.localOffset.z + settings.worldOffset.z) * frequency
            );

            totalAmplitude += amplitude;
            
            amplitude *= settings.amplitudeMultiplyer;
            frequency *= settings.frequencyMultyplier;
        }

        return result/totalAmplitude;

    }

    public static float OctaveNoise2D(Vector2Int pos, NoiseSettings settings){

        float x = (float)pos.x * settings.noiseZoom.x;
        float z = (float)pos.y * settings.noiseZoom.z;

        x += settings.noiseZoom.x;
        z += settings.noiseZoom.z;


        float result = 0;
        float frequency = 1;
        float amplitude = 1;

        float totalAmplitude = 0;
        float noiseResult;

        for(int i = 0; i < settings.octaves; i++){

            //result += Mathf.PerlinNoise((x + settings.perlinOffset.x + settings.worldOffset.x) * frequency, (z + settings.perlinOffset.z + settings.worldOffset.z) * frequency) * amplitude;

            noiseResult = Noise2D((x + settings.localOffset.x + settings.worldOffset.x) * frequency, (z + settings.localOffset.z + settings.worldOffset.z) * frequency);

            result +=  noiseResult * amplitude;
           

            totalAmplitude += amplitude;
            
            amplitude *= settings.amplitudeMultiplyer;
            frequency *= settings.frequencyMultyplier;
        }

        return result/totalAmplitude;


    }

    public static float ResistributeNoise(float noise, NoiseSettings settings){
        noise = Redistribution(noise, settings);

        float minRedistribution = 0;
        float maxRedistribution = Redistribution(1, settings);

        noise = MapFloatValue(noise, minRedistribution, maxRedistribution, 0, 1);

        return noise;
    }

    public static float MapFloatValue(float input, float inputMinRange, float inputMaxRange, float outputMinRange, float outputMaxRange){
        return outputMinRange + (input - inputMinRange) * (outputMaxRange - outputMinRange) / (inputMaxRange - inputMinRange);
    }

    public static int MapIntValue(int input, int inputMinRange, int inputMaxRange, int outputMinRange, int outputMaxRange){
        return outputMinRange + (input - inputMinRange) * (outputMaxRange - outputMinRange) / (inputMaxRange - inputMinRange);
    }

    public static float MapNormalizedValue(float input, float outputMinRange, float outputMaxRange){
        return MapFloatValue(input, 0, 1, outputMinRange, outputMaxRange);
    }

    public static int MapNormalizedValueToInt(float input, int outputMinRange, int outputMaxRange){
        return (int)MapNormalizedValue(input, outputMinRange, outputMaxRange);
    }

    public static float Redistribution(float noise, NoiseSettings settings){
        return Mathf.Pow(noise * settings.redistributionModifier, settings.exponent);
    }


}
