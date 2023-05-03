using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainWarping : MonoBehaviour {
    public NoiseSettings noiseDomainX, noiseDomainY, noiseDomainZ;
    public Vector3 amplitude = new Vector3Int(1, 1, 1);

    private FastNoiseLite noise = new FastNoiseLite();
    public FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
    public FastNoiseLite.DomainWarpType domainWarpType = FastNoiseLite.DomainWarpType.OpenSimplex2;
    public void Awake(){
        noise.SetNoiseType(noiseType);
        noise.SetDomainWarpType(domainWarpType);
    }

    public void OnValidate(){
        noise.SetNoiseType(noiseType);
        noise.SetDomainWarpType(domainWarpType);
    }

    public float GenerateDomainNoise2D(Vector2Int pos, NoiseSettings settings){

        Vector2Int domainOffset = GenerateDomainOffsetInt2D(pos);
        return CustomNoise.OctaveNoise2D(
            new Vector2Int(domainOffset.x, domainOffset.y), 
            settings);

    }

    public Vector2 GenerateDomainOffset2D(Vector2Int pos){

        float x = (float)pos.x;
        float z = (float)pos.y;

        noise.DomainWarp(ref x, ref z);

        float noiseX = amplitude.x * x;
        float noiseZ = amplitude.z * z;

        return new Vector2(noiseX, noiseZ);

    }

    public Vector2Int GenerateDomainOffsetInt2D(Vector2Int pos){
        return Vector2Int.RoundToInt(GenerateDomainOffset2D(pos));
    }

    


    public float GenerateDomainNoise3D(Vector3Int pos, NoiseSettings settings){

        Vector3Int domainOffset = GenerateDomainOffsetInt3D(pos);
        return CustomNoise.OctaveNoise3D(
            new Vector3Int(domainOffset.x, domainOffset.y, domainOffset.z), 
            settings);

    }

    public Vector3 GenerateDomainOffset3D(Vector3Int pos){

        float x = (float)pos.x;
        float y = (float)pos.y;
        float z = (float)pos.z;

        noise.DomainWarp(ref x, ref y, ref z);

        float noiseX = amplitude.x * x;
        float noiseY = amplitude.y * y;
        float noiseZ = amplitude.z * z;

        return new Vector3(noiseX, noiseY, noiseZ);

    }

    public Vector3Int GenerateDomainOffsetInt3D(Vector3Int pos){
        return Vector3Int.RoundToInt(GenerateDomainOffset3D(pos));
    }


}
