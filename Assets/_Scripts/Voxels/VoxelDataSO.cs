using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Voxel Data", menuName = "Data/Voxel Data")]
public class VoxelDataSO : ScriptableObject{

    public float textureSizeX, textureSizeY;
    public List<TextureData> textureDataList;
    
}

[System.Serializable]
public class TextureData{

    public VoxelType voxelType;
    public Vector2Int up, down, side;
    public bool isSolid = true;
    public bool generatesCollider = true;
    public bool isLiquid = false;
    public bool isDestructable = false;

}