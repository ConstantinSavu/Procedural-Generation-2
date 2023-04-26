using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomVoxelLayer : VoxelLayerHandler
{
    
    [Range(0,1)]
    public float customVoxelThreshold = 0.2f;
    public VoxelType customVoxelType;

    [SerializeField]
    private NoiseSettings customVoxelSettings;

    public DomainWarping domainWarping;

    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight){

        if(Chunk.GetVoxelFromChunkCoordinates(data, pos) == VoxelType.Bedrock){
            return false;
        }

        VoxelType voxelType;

        int worldPosY = pos.y + data.worldPosition.y;

        if(worldPosY > surfaceHeight){
            return false;
        }

        customVoxelSettings.worldOffset = data.worldReference.worldData.worldSettings.mapSeedOffset;
        
        // float customVoxelNoise = CustomNoise.OctavePerlin2D(
        //     new Vector2Int(
        //         data.worldPosition.x + pos.x,
        //         data.worldPosition.z + pos.z
        //     ),
        //     customVoxelSettings
        // );
        
        float customVoxelNoise = domainWarping.GenerateDomainNoise3D(
            new Vector3Int(
                data.worldPosition.x + pos.x,
                pos.y + data.worldPosition.y,
                data.worldPosition.z + pos.z
            ),
            customVoxelSettings
        );

        if(customVoxelNoise < customVoxelThreshold){
            return false;
        }

        voxelType = customVoxelType;

        Chunk.SetVoxelFromChunkCoordinates(data, pos, voxelType);
        return true;
    }

}
