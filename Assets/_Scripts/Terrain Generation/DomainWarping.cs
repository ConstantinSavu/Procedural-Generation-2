using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainWarping : MonoBehaviour {
    public NoiseSettings noiseDomainX, noiseDomainY, noiseDomainZ;
    public Vector3Int amplitude = new Vector3Int(20, 20, 20);

    public float GenerateDomainNoise2D(Vector2Int pos, NoiseSettings settings){

        Vector2Int domainOffset = GenerateDomainOffsetInt2D(pos);
        return CustomNoise.OctavePerlin2D(
            new Vector2Int(pos.x + domainOffset.x, pos.y + domainOffset.y), 
            settings);

    }

    public Vector2 GenerateDomainOffset2D(Vector2Int pos){

        float noiseX = amplitude.x * CustomNoise.OctavePerlin2D(
            new Vector2Int(pos.x, pos.y), 
            noiseDomainX);
        float noiseZ = amplitude.z * CustomNoise.OctavePerlin2D(
            new Vector2Int(pos.x, pos.y),
            noiseDomainZ);

        return new Vector2(noiseX, noiseZ);

    }

    public Vector2Int GenerateDomainOffsetInt2D(Vector2Int pos){
        return Vector2Int.RoundToInt(GenerateDomainOffset2D(pos));
    }

    


    public float GenerateDomainNoise3D(Vector3Int pos, NoiseSettings settings){

        Vector3Int domainOffset = GenerateDomainOffsetInt3D(pos);
        return CustomNoise.OctavePerlin3D(
            new Vector3Int(pos.x + domainOffset.x, pos.y + domainOffset.y, pos.z + domainOffset.z), 
            settings);

    }

    public Vector3 GenerateDomainOffset3D(Vector3Int pos){

        float noiseX = amplitude.x * CustomNoise.OctavePerlin3D(
            new Vector3Int(pos.x, pos.y, pos.z), 
            noiseDomainX);
        
        float noiseY = amplitude.y * CustomNoise.OctavePerlin3D(
            new Vector3Int(pos.x, pos.y, pos.z), 
            noiseDomainX);

        float noiseZ = amplitude.z * CustomNoise.OctavePerlin3D(
            new Vector3Int(pos.x, pos.y, pos.z),
            noiseDomainZ);

        return new Vector3(noiseX, noiseY, noiseZ);

    }

    public Vector3Int GenerateDomainOffsetInt3D(Vector3Int pos){
        return Vector3Int.RoundToInt(GenerateDomainOffset3D(pos));
    }


}
