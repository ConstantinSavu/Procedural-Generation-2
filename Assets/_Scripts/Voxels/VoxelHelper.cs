using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoxelHelper
{
    public enum Direction {
        up,
        down,
        right,
        left,
        forward,
        back
    }

    public enum DrawDirection {
        positive,
        negative
    }

    public static Direction[] directions = {

        Direction.up,
        Direction.down,
        Direction.right,
        Direction.left,
        Direction.forward,
        Direction.back

    };

    public static Vector3Int GetDirectionVector(Direction direction){
        return direction switch{
            Direction.up        => Vector3Int.up,
            Direction.down      => Vector3Int.down,
            Direction.right     => Vector3Int.right,
            Direction.left      => Vector3Int.left,
            Direction.forward   => Vector3Int.forward,
            Direction.back      => Vector3Int.back,
            _                   => throw new Exception("Invalid input") 
        };
    }


    public static Vector2Int TexturePosition(Direction direction, VoxelType voxelType){
        return direction switch
        {
            Direction.up => VoxelDataManager.voxelTextureDataDictionary[voxelType].up,
            Direction.down => VoxelDataManager.voxelTextureDataDictionary[voxelType].down,
            _ => VoxelDataManager.voxelTextureDataDictionary[voxelType].side
        };
    }

    public static MeshData GetVoxelMeshData(ChunkData chunk, Vector3Int pos, MeshData meshData, VoxelType voxelType){

        if(voxelType == VoxelType.Air || voxelType == VoxelType.Nothing){
            return meshData;
        }
        
        foreach(Direction direction in directions){

            Vector3Int neighbourVoxelCoordinates = pos + GetDirectionVector(direction);
            VoxelType neighbourVoxelType =  Chunk.GetVoxelFromChunkCoordinates(chunk, neighbourVoxelCoordinates);

            if(VoxelDataManager.voxelTextureDataDictionary[neighbourVoxelType].isSolid){
                
                continue;
            }

            if(VoxelDataManager.voxelTextureDataDictionary[neighbourVoxelType] == 
                VoxelDataManager.voxelTextureDataDictionary[voxelType]){
                
                continue;
            }

            if(neighbourVoxelType == VoxelType.Nothing){
                
                continue;
            }

            meshData = MeshGenerationLogic(direction, chunk, pos, meshData, voxelType, neighbourVoxelType);

        }

        return meshData;
    }

    public static MeshData MeshGenerationLogic(Direction direction, ChunkData chunk, Vector3Int normalPos, MeshData meshData, VoxelType voxelType, VoxelType neighbourVoxelType){
        
        Vector3 voxelSize = chunk.worldReference.worldSettings.voxelSize;

        Vector3 pos = Vector3.Scale(normalPos, voxelSize);

        Vector3 defaultPositiveOffset = Vector3.Scale(new Vector3(0.5f, 0.5f, 0.5f), voxelSize);
        Vector3 defaultNegativeOffset = Vector3.Scale(new Vector3(-0.5f, -0.5f, -0.5f), voxelSize);

        Vector3 positiveDrawLowerOffset = Vector3.Scale(new Vector3(0.5f, 0.35f, 0.5f), voxelSize);
        Vector3 negativeDrawLowerOffset = Vector3.Scale(new Vector3(-0.5f, 0.35f, -0.5f), voxelSize);

        Vector3 positiveOffset = defaultPositiveOffset;
        Vector3 negativeOffset = defaultNegativeOffset;

        Vector2 defaultPositiveTextureOffset = new Vector2(0.5f, 0.5f);
        Vector2 defaultNegativeTextureOffset = new Vector2(-0.5f, -0.5f);

        Vector2 defaultPositiveDrawLowerTextureOffset = new Vector2(0.5f, 0.35f);
        Vector2 defaultNegativeDrawLowerTextureOffset = new Vector2(-0.5f, 0.35f);

        Vector2 positiveTextureOffset = defaultPositiveTextureOffset;
        Vector2 negativeTextureOffset = defaultNegativeTextureOffset;
        
        Vector3Int neighbourVoxelCoordinates;
        Vector3Int upNeighbourVoxelCoordinates;
        VoxelType upNeighbour;

        if(VoxelDataManager.voxelTextureDataDictionary[voxelType].isLiquid){

            upNeighbourVoxelCoordinates = normalPos + Vector3Int.up;
            upNeighbour = Chunk.GetVoxelFromChunkCoordinates(chunk, upNeighbourVoxelCoordinates);

            if(VoxelDataManager.voxelTextureDataDictionary[upNeighbour].voxelType == VoxelType.Air){
               positiveOffset = positiveDrawLowerOffset; 
            }

            meshData.liquidMesh = GetFaceDataIn(direction, chunk, pos, meshData.liquidMesh, voxelType, positiveOffset, negativeOffset, positiveTextureOffset, negativeTextureOffset);
            return meshData;

        }

        

        if(!VoxelDataManager.voxelTextureDataDictionary[neighbourVoxelType].isLiquid){

            if(VoxelDataManager.voxelTextureDataDictionary[voxelType].isTransparent){
                meshData.transparentMesh = GetFaceDataIn(direction, chunk, pos, meshData.transparentMesh, voxelType, positiveOffset, negativeOffset, positiveTextureOffset, negativeTextureOffset);
                return meshData;
            }

            meshData.solidMesh = GetFaceDataIn(direction, chunk, pos, meshData.solidMesh, voxelType, positiveOffset, negativeOffset, positiveTextureOffset, negativeTextureOffset);
            return meshData;
        }

        neighbourVoxelCoordinates = normalPos + GetDirectionVector(direction);
        upNeighbourVoxelCoordinates = neighbourVoxelCoordinates + Vector3Int.up;
        upNeighbour = Chunk.GetVoxelFromChunkCoordinates(chunk, upNeighbourVoxelCoordinates);

        if(VoxelDataManager.voxelTextureDataDictionary[upNeighbour].voxelType != VoxelType.Air){
            
            meshData.underLiquidMesh = GetFaceDataIn(direction, chunk, pos, meshData.underLiquidMesh, voxelType, positiveOffset, negativeOffset, positiveTextureOffset, negativeTextureOffset);
            return meshData;
        }
        

        if(direction == Direction.up || direction == Direction.down){
            meshData.underLiquidMesh = GetFaceDataIn(direction, chunk, pos, meshData.underLiquidMesh, voxelType, positiveOffset, negativeOffset, positiveTextureOffset, negativeTextureOffset);
            return meshData; 
        }

        positiveOffset = defaultPositiveOffset;
        negativeOffset = negativeDrawLowerOffset;

        positiveTextureOffset = defaultPositiveTextureOffset;
        negativeTextureOffset = defaultNegativeDrawLowerTextureOffset;
        

        if(VoxelDataManager.voxelTextureDataDictionary[voxelType].isTransparent){
            meshData.transparentMesh = GetFaceDataIn(direction, chunk, pos, meshData.transparentMesh, voxelType, positiveOffset, negativeOffset, positiveTextureOffset, negativeTextureOffset);
            
        }
        else{
            meshData.solidMesh = GetFaceDataIn(direction, chunk, pos, meshData.solidMesh, voxelType, positiveOffset, negativeOffset, positiveTextureOffset, negativeTextureOffset);
        }


        positiveOffset = positiveDrawLowerOffset;
        negativeOffset = defaultNegativeOffset;

        positiveTextureOffset = defaultPositiveDrawLowerTextureOffset;
        negativeTextureOffset = defaultNegativeTextureOffset;

        meshData.underLiquidMesh = GetFaceDataIn(direction, chunk, pos, meshData.underLiquidMesh, voxelType, positiveOffset, negativeOffset, positiveTextureOffset, negativeTextureOffset);

        return meshData;
    }
    public static MeshData GetFaceDataIn(Direction direction, ChunkData chunk, Vector3 pos, MeshData meshData, VoxelType voxelType, Vector3 positiveOffset, Vector3 negativeOffset, Vector2 texturePositiveOffset, Vector2 textureNegativeOffset){

        bool generatesCollider = VoxelDataManager.voxelTextureDataDictionary[voxelType].generatesCollider;
        bool isLiquid = VoxelDataManager.voxelTextureDataDictionary[voxelType].isLiquid;

        GetFaceVertices(direction, chunk, pos, meshData, voxelType, positiveOffset, negativeOffset);
        meshData.AddQuadTriangles(generatesCollider);
        meshData.uv.AddRange(FaceUVs(direction, voxelType, texturePositiveOffset, textureNegativeOffset));

        return meshData;
    }

    public static void GetFaceVertices(Direction direction, ChunkData chunk, Vector3 pos, MeshData meshData, VoxelType voxelType, Vector3 positiveOffset, Vector3 negativeOffset)
    {
        float x = pos.x;
        float y = pos.y;
        float z = pos.z;

    
        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.back:
                
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + negativeOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + positiveOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + positiveOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + negativeOffset.y, z + negativeOffset.z));
                
                    
                break;
            case Direction.forward:
                
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + negativeOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + positiveOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + positiveOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + negativeOffset.y, z + positiveOffset.z));
                
                break;
            case Direction.left:

                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + negativeOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + positiveOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + positiveOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + negativeOffset.y, z + negativeOffset.z));
                
                break;

            case Direction.right:
    
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + negativeOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + positiveOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + positiveOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + negativeOffset.y, z + positiveOffset.z));
                
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + negativeOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + negativeOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + negativeOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + negativeOffset.y, z + positiveOffset.z));

                break;
            case Direction.up:
        
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + positiveOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + positiveOffset.y, z + positiveOffset.z));
                meshData.AddVertex(new Vector3(x + positiveOffset.x, y + positiveOffset.y, z + negativeOffset.z));
                meshData.AddVertex(new Vector3(x + negativeOffset.x, y + positiveOffset.y, z + negativeOffset.z));
            
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, VoxelType voxelType, Vector3 positiveOffset, Vector3 negativeOffset){

        Vector2[] UVs = new Vector2[4];
        Vector2 tilePos = TexturePosition(direction, voxelType) + Vector2.one * 0.5f;

        Vector2 southEastPos = new Vector2(tilePos.x + positiveOffset.x, tilePos.y + negativeOffset.y);
        Vector2 southEastOffset = Vector2.left + Vector2.up;

        Vector2 northEastPos =  new Vector2(tilePos.x + positiveOffset.x, tilePos.y + positiveOffset.y);
        Vector2 northEastOffset = Vector2.left + Vector2.down;

        Vector2 northWest =  new Vector2(tilePos.x + negativeOffset.x, tilePos.y + positiveOffset.y);
        Vector2 northWestOffset = Vector2.right + Vector2.down;

        Vector2 southWest =  new Vector2(tilePos.x + negativeOffset.x, tilePos.y + negativeOffset.y);
        Vector2 southWestOffset = Vector2.right + Vector2.up;

        UVs[0] = Vector2.Scale(southEastPos, VoxelDataManager.tileSize) + VoxelDataManager.textureOffset * southEastOffset;
        UVs[1] = Vector2.Scale(northEastPos, VoxelDataManager.tileSize) + VoxelDataManager.textureOffset * northEastOffset;
        UVs[2] = Vector2.Scale(northWest, VoxelDataManager.tileSize) + VoxelDataManager.textureOffset * northWestOffset;
        UVs[3] = Vector2.Scale(southWest, VoxelDataManager.tileSize) + VoxelDataManager.textureOffset * southWestOffset;

        return UVs;

    }

}
