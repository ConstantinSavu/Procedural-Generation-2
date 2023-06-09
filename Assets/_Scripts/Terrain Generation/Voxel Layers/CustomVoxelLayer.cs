using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomVoxelLayer : VoxelLayerHandler
{
    
    
    public float customVoxelThreshold = 0.2f;
    private float maxCustomVoxelThreshold = 1000;

    public bool invertNoise = false;

    private float normalizedCustomVoxelThreshold;
    public VoxelType customVoxelType;

    [SerializeField]
    private NoiseSettings customVoxelSettings;

    public DomainWarping domainWarping;

    public bool useDomainWarping = false;


    public void Awake(){
        canReplaceHashSet = new HashSet<VoxelType>(canReplace);
        cannotReplaceHashSet = new HashSet<VoxelType>(cannotReplace);
        normalizedCustomVoxelThreshold = customVoxelThreshold / maxCustomVoxelThreshold;
    }

    protected override bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight, ref VoxelType currentVoxel){

        

        if(cannotReplaceHashSet.Contains(currentVoxel)){
            return false;
        }

        if(canReplaceHashSet.Count != 0){

            if(!canReplaceHashSet.Contains(currentVoxel)){
                return false;
            }

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

            customVoxelNoise = CustomNoise.Redistribution(customVoxelNoise, customVoxelSettings);
        }


        if(!invertNoise){
            if(customVoxelNoise < normalizedCustomVoxelThreshold){
            return false;
            }
        }
        else{
            if(customVoxelNoise > normalizedCustomVoxelThreshold){
            return false;
            }
        }
        
        
        voxelType = customVoxelType;
        currentVoxel = voxelType;

        Chunk.SetVoxelFromChunkCoordinates(data, pos, voxelType);
        
        return true;
    }

    private void OnValidate() {

        canReplaceHashSet = new HashSet<VoxelType>(canReplace);
        cannotReplaceHashSet = new HashSet<VoxelType>(cannotReplace);

        if(customVoxelThreshold < 0){
            customVoxelThreshold = 0;
        }

        if(customVoxelThreshold > maxCustomVoxelThreshold){
            customVoxelThreshold = maxCustomVoxelThreshold;
        }

        normalizedCustomVoxelThreshold = customVoxelThreshold / maxCustomVoxelThreshold;

    }

}
