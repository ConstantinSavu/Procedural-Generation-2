using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldSettings", menuName = "Data/WorldSettings")]

public class WorldSettings : ScriptableObject
{
    public Vector3Int startingPosition = new Vector3Int(0, 0, 0);
    public Vector3Int chunkSize = new Vector3Int(Constants.chunkWidth, Constants.chunkHeight, Constants.chunkDepth);
    public Vector3 voxelSize = new Vector3(1f, 1f, 1f);
    [HideInInspector] public Vector3 inverseVoxelSize;
    public Vector3Int chunkDrawingRange = new Vector3Int(8, 8, 8);

    public Vector3Int mapSeedOffset;
    public int waterThreshold = Constants.waterThreshold;

    public Vector3Int voxelMinMapDimensions;
    public Vector3Int voxelMaxMapDimensions;

    [HideInInspector] 
    public Vector3 minMapDimensions;
    [HideInInspector] 
    public Vector3 maxMapDimensions;
    
    
}
