using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldSettings", menuName = "Data/WorldSettings")]

public class WorldSettings : ScriptableObject
{
    public Vector3Int chunkSize = new Vector3Int(Constants.chunkWidth, Constants.chunkHeight, Constants.chunkDepth);
    public Vector3Int chunkDrawingRange = new Vector3Int(8, 8, 8);

    public Vector3Int mapSeedOffset;
    public int waterThreshold = Constants.waterThreshold;

    //[HideInInspector] 
    public Vector3Int voxelMinMapDimensions;
    public Vector3Int VoxelMinMapDimensions{
        get; set;
    }

    //[HideInInspector] 
    public Vector3Int voxelMaxMapDimensions;
    public Vector3Int VoxelMaxMapDimensions{
        get; set;
    }
    
}
