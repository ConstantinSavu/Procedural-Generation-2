using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomNoise
{
    public static float PerlinNoise3D(Vector3 pos){

        float xy = Mathf.PerlinNoise(pos.x, pos.y);
        float xz = Mathf.PerlinNoise(pos.x, pos.z);
        float yz = Mathf.PerlinNoise(pos.y, pos.z);
        float yx = Mathf.PerlinNoise(pos.y, pos.x);
        float zx = Mathf.PerlinNoise(pos.z, pos.x);
        float zy = Mathf.PerlinNoise(pos.z, pos.y);
    
        return (xy + xz + yz + yx + zx + zy) / 6.0f;
    }

    public static float OctavePerlin3D(Vector3Int pos, NoiseSettings settings){

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

            result += amplitude * PerlinNoise3D(new Vector3(
                (x + settings.perlinOffset.x + settings.worldOffset.x) * frequency,
                (y + settings.perlinOffset.y + settings.worldOffset.y) * frequency, 
                (z + settings.perlinOffset.z + settings.worldOffset.z) * frequency
            
            ));

            totalAmplitude += amplitude;
            
            amplitude *= settings.amplitudeMultiplyer;
            frequency *= settings.frequencyMultyplier;
        }

        return result/totalAmplitude;

    }

    public static float OctavePerlin2D(Vector2Int pos, NoiseSettings settings){

        float x = (float)pos.x * settings.noiseZoom.x;
        float z = (float)pos.y * settings.noiseZoom.z;

        x += settings.noiseZoom.x;
        z += settings.noiseZoom.z;


        float result = 0;
        float frequency = 1;
        float amplitude = 1;

        float totalAmplitude = 0;

        for(int i = 0; i < settings.octaves; i++){

            result += Mathf.PerlinNoise((x + settings.perlinOffset.x + settings.worldOffset.x) * frequency, (z + settings.perlinOffset.z + settings.worldOffset.z) * frequency) * amplitude;

            totalAmplitude += amplitude;
            
            amplitude *= settings.amplitudeMultiplyer;
            frequency *= settings.frequencyMultyplier;
        }

        return result/totalAmplitude;


    }

    public static float ResistributeNoise(float noise, NoiseSettings settings){
        noise = Redistribution(noise, settings);

        float minRedistribution = 0;
        float maxRedistribution = CustomNoise.Redistribution(1, settings);

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
