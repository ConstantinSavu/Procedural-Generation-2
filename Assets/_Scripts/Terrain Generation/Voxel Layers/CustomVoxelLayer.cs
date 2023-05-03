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

    public bool useDomainWarping = false;

    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight){

        VoxelType currentVoxel = Chunk.GetVoxelFromChunkCoordinates(data, pos);

        if(currentVoxel == VoxelType.Bedrock){
            return false;
        }

        if(currentVoxel == VoxelType.Water){
            return false;
        }

        if(currentVoxel == VoxelType.Sand){
           return false; 
        }

        VoxelType voxelType;

        int worldPosY = pos.y + data.worldPosition.y;

        if(worldPosY > surfaceHeight){
            return false;
        }

        customVoxelSettings.worldOffset = data.worldReference.worldData.worldSettings.mapSeedOffset;
        float customVoxelNoise;
    
        if(useDomainWarping){

        
            customVoxelNoise = domainWarping.GenerateDomainNoise3D(
                new Vector3Int(
                    data.worldPosition.x + pos.x,
                    pos.y + data.worldPosition.y,
                    data.worldPosition.z + pos.z
                ),
                customVoxelSettings
            );
        }
        else{
            customVoxelNoise = CustomNoise.OctaveNoise3D(
            new Vector3Int(
            data.worldPosition.x + pos.x,
            data.worldPosition.y + pos.y,
            data.worldPosition.z + pos.z
            ),
            customVoxelSettings
            );
        }

        if(customVoxelNoise < customVoxelThreshold){
            return false;
        }

        voxelType = customVoxelType;

        Chunk.SetVoxelFromChunkCoordinates(data, pos, voxelType);
        return true;
    }

}
