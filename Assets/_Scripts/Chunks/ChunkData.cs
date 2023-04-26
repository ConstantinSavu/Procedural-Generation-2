using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{

    public VoxelType[] voxels;
 
    public Vector3Int chunkSize = new Vector3Int(Constants.chunkWidth, Constants.chunkHeight, Constants.chunkDepth);
    public World worldReference;
    public Vector3Int worldPosition;
    public NoiseSettings noiseSettings;

    public Dictionary<Vector3Int, VoxelType> outOfChunkBoundsVoxelDictionary = new Dictionary<Vector3Int, VoxelType>();

    public bool modifiedByPlayer;

    public ChunkData(Vector3Int chunkSize, World world, Vector3Int worldPosition){

        this.chunkSize = chunkSize;
        this.worldReference = world;
        this.worldPosition = worldPosition;
        this.voxels = new VoxelType[chunkSize.x * chunkSize.y * chunkSize.z];


    }
    
}
