using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f;
    public static Vector2 tileSize;
    public static Dictionary<VoxelType, TextureData> voxelTextureDataDictionary = new Dictionary<VoxelType, TextureData>();
    public VoxelDataSO textureData;


    private void Awake(){
        foreach(TextureData item in textureData.textureDataList){

            if(voxelTextureDataDictionary.ContainsKey(item.voxelType)){
                continue;
            }

            voxelTextureDataDictionary.Add(item.voxelType, item);

        }

        tileSize.x = textureData.textureSizeX;
        tileSize.y = textureData.textureSizeY;

    }

}
