using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Chunk
{

    public static void LoopThroughVoxels(ChunkData chunkData, Action<Vector3Int> actionToPerform){

        for(int index = 0; index < chunkData.voxels.Length; index++){
            
            Vector3Int position = GetPositionFromIndex(chunkData, index);
            actionToPerform(new Vector3Int(position.x, position.y, position.z));
        };

    }

    public static List<ChunkData> GetEdgeNeighbourChunk(ChunkData chunkData, Vector3Int worldPos){

        Vector3Int chunkPos = GetVoxelInChunkCoordinates(chunkData, worldPos);
        List<ChunkData> neighbourDataList = new List<ChunkData>();
        
        if(chunkPos.x == 0){
            neighbourDataList.Add(WorldDataHelper.GetChunkDataFromWorldCoordinates(chunkData.worldReference, worldPos - Vector3Int.right));
        }
        else if(chunkPos.x == chunkData.chunkSize.x - 1){
            neighbourDataList.Add(WorldDataHelper.GetChunkDataFromWorldCoordinates(chunkData.worldReference, worldPos + Vector3Int.right));
        }

        if(chunkPos.y == 0){
            neighbourDataList.Add(WorldDataHelper.GetChunkDataFromWorldCoordinates(chunkData.worldReference, worldPos - Vector3Int.up));
        }
        else if(chunkPos.y == chunkData.chunkSize.y - 1){
            neighbourDataList.Add(WorldDataHelper.GetChunkDataFromWorldCoordinates(chunkData.worldReference, worldPos + Vector3Int.up));
        }

        if(chunkPos.z == 0){
            neighbourDataList.Add(WorldDataHelper.GetChunkDataFromWorldCoordinates(chunkData.worldReference, worldPos - Vector3Int.forward));
        }
        else if(chunkPos.z == chunkData.chunkSize.z - 1){
            neighbourDataList.Add(WorldDataHelper.GetChunkDataFromWorldCoordinates(chunkData.worldReference, worldPos + Vector3Int.forward));
        }

        return neighbourDataList;

    }

    public static bool VoxelIsOnEdge(ChunkData chunkData, Vector3Int worldPos){

        Vector3Int chunkPos = GetVoxelInChunkCoordinates(chunkData, worldPos);

        if(chunkPos.x == 0 || chunkPos.x == chunkData.chunkSize.x - 1){
            return true;
        }

        if(chunkPos.y == 0 || chunkPos.y == chunkData.chunkSize.y - 1){
            return true;
        }

        if(chunkPos.z == 0 || chunkPos.z == chunkData.chunkSize.z - 1){
            return true;
        }

        return false;
    }

    public static bool InRange(ChunkData chunkData, Vector3Int localPos)
    {
        if (localPos.x < 0)
        {
            return false;
        }

        if (localPos.x >= chunkData.chunkSize.x)
        {
            return false;
        }

        if (localPos.y < 0)
        {
            return false;
        }

        if (localPos.y >= chunkData.chunkSize.y)
        {
            return false;
        }

        if (localPos.z < 0)
        {
            return false;
        }

        if (localPos.z >= chunkData.chunkSize.z)
        {
            return false;
        }

        return true;

    }

    public static VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, Vector3Int localPosition){

        int index;

        //voxel not in range so switch to worldPosition
        if (!InRange(chunkData, localPosition))
        {
            return WorldDataHelper.GetVoxelFromWorldCoorinates(chunkData.worldReference, chunkData.worldPosition + localPosition);
        }

        index = GetIndexFromPosition(chunkData, localPosition);
        return chunkData.voxels[index];
        

    }

    public static Vector3Int ChunkPositionFromVoxelWorldCoordinates(ChunkData chunkData, Vector3Int worldPos)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(worldPos.x / (float)chunkData.worldReference.worldSettings.chunkSize.x) * chunkData.worldReference.worldSettings.chunkSize.x,
            y = Mathf.FloorToInt(worldPos.y / (float)chunkData.worldReference.worldSettings.chunkSize.y) * chunkData.worldReference.worldSettings.chunkSize.y,
            z = Mathf.FloorToInt(worldPos.z / (float)chunkData.worldReference.worldSettings.chunkSize.z) * chunkData.worldReference.worldSettings.chunkSize.z
        };
    }

    public static bool SetVoxelFromChunkCoordinates(ChunkData chunkData, Vector3Int localPosition, VoxelType voxelType){

        int index;


        //out of range so use worldPosition
        if (!InRange(chunkData, localPosition))
        {
            Vector3Int worldPosition = chunkData.worldPosition + localPosition;
            bool res = WorldDataHelper.SetVoxelFromWorldCoordinates(chunkData.worldReference, worldPosition, voxelType);

            if(res == true){
                return true;
            }

            if(chunkData.outOfChunkBoundsVoxelDictionary.ContainsKey(worldPosition)){
                chunkData.outOfChunkBoundsVoxelDictionary[worldPosition] = voxelType;
            }
            else{
                chunkData.outOfChunkBoundsVoxelDictionary.Add(worldPosition, voxelType);
            }

            return false;

        }

        index = GetIndexFromPosition(chunkData, localPosition);
        chunkData.voxels[index] = voxelType;
        return true;
    }

    private static Vector3Int GetPositionFromIndex(ChunkData chunkData, int index){

        int x = index % chunkData.chunkSize.x;
        int y = (index / chunkData.chunkSize.x) % chunkData.chunkSize.y;
        int z = index / (chunkData.chunkSize.x * chunkData.chunkSize.y);


        return new Vector3Int(x, y, z);
    }

    private static int GetIndexFromPosition(ChunkData chunkData, Vector3Int localPosition){

        return localPosition.x + chunkData.chunkSize.x * localPosition.y + chunkData.chunkSize.x * chunkData.chunkSize.y * localPosition.z;

    }

    public static Vector3Int GetVoxelInChunkCoordinates(ChunkData chunkData, Vector3Int pos){

        return new Vector3Int {
            
            x = pos.x - chunkData.worldPosition.x,
            y = pos.y - chunkData.worldPosition.y,
            z = pos.z - chunkData.worldPosition.z

        };

    }

    public static Vector3Int VoxelPosition(ChunkData chunkData, Vector3Int pos){

        return new Vector3Int {
            
            x = pos.x + chunkData.worldPosition.x,
            y = pos.y + chunkData.worldPosition.y,
            z = pos.z + chunkData.worldPosition.z

        };

    }

    public static MeshData GetChunkMeshData(ChunkData chunkData){
        
        MeshData meshData = new MeshData(true);

        LoopThroughVoxels(chunkData, pos => meshData = VoxelHelper.GetVoxelMeshData(chunkData, pos, meshData, chunkData.voxels[GetIndexFromPosition(chunkData, pos)]));

        return meshData;

    }

    public static Vector3Int ChunkPositionFromVoxelCoords(World world, Vector3Int pos){

        return new Vector3Int(
            Mathf.FloorToInt(pos.x / (float)world.worldData.worldSettings.chunkSize.x) * world.worldData.worldSettings.chunkSize.x,
            Mathf.FloorToInt(pos.y / (float)world.worldData.worldSettings.chunkSize.y) * world.worldData.worldSettings.chunkSize.y,
            Mathf.FloorToInt(pos.z / (float)world.worldData.worldSettings.chunkSize.z) * world.worldData.worldSettings.chunkSize.z
        );

    }
}
