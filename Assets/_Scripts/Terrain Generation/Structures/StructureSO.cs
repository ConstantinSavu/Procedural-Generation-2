using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "Data/Strucutre Data")]

public class StructureSO : ScriptableObject
{
    public List<StructureVoxel> strucutreList;
}

[System.Serializable]
public class StructureVoxel
{
    public VoxelType voxelType;
    public Vector3Int voxelPosition;
}