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

    private static Direction[] directions = {

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

    public static MeshData GetMeshData(ChunkData chunk, Vector3Int pos, MeshData meshData, VoxelType voxelType){

        if(voxelType == VoxelType.Air || voxelType == VoxelType.Nothing){
            return meshData;
        }
        
        foreach(Direction direction in directions){

            Vector3Int neighbourVoxelCoordinates = pos + GetDirectionVector(direction);
            VoxelType neighbourVoxelType =  Chunk.GetVoxelFromChunkCoordinates(chunk, neighbourVoxelCoordinates);

            if(VoxelDataManager.voxelTextureDataDictionary[neighbourVoxelType].isSolid){
                
                continue;
            }

            if(neighbourVoxelType == VoxelType.Nothing){
                
                continue;
            }


            
            if(voxelType != VoxelType.Water){
                
                meshData = GetFaceDataIn(direction, chunk, pos, meshData, voxelType);
            }
            else if(neighbourVoxelType != VoxelType.Water){
                meshData.waterMesh = GetFaceDataIn(direction, chunk, pos, meshData.waterMesh, voxelType);
            }


        }

        return meshData;
    }

    public static MeshData GetFaceDataIn(Direction direction, ChunkData chunk, Vector3Int pos, MeshData meshData, VoxelType voxelType){

        bool generatesCollider = VoxelDataManager.voxelTextureDataDictionary[voxelType].generatesCollider;
        bool isLiquid = VoxelDataManager.voxelTextureDataDictionary[voxelType].isLiquid;

        GetFaceVertices(direction, pos, meshData, voxelType);
        meshData.AddQuadTriangles(generatesCollider, isLiquid);
        meshData.uv.AddRange(FaceUVs(direction, voxelType));

        return meshData;

    }

    public static void GetFaceVertices(Direction direction, Vector3Int pos, MeshData meshData, VoxelType voxelType)
    {
        bool generatesCollider = VoxelDataManager.voxelTextureDataDictionary[voxelType].generatesCollider;
        bool isLiquid = VoxelDataManager.voxelTextureDataDictionary[voxelType].isLiquid;

        int x = pos.x;
        int y = pos.y;
        int z = pos.z;
        
        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.back:
                if(isLiquid){
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.35f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.35f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                }
                else{
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                }
                    
                break;
            case Direction.forward:
                if(isLiquid){
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.35f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.35f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                }
                else{
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                }
                break;
            case Direction.left:
                if(isLiquid){
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.35f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.35f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                }
                else{

                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                }   
                break;

            case Direction.right:
                if(isLiquid){
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.35f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.35f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                }
                else{
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                }
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.up:
                if(isLiquid){
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.35f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.35f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.35f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.35f, z - 0.5f), generatesCollider);
                }
                else{
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                }
                
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, VoxelType voxelType){

        Vector2[] UVs = new Vector2[4];
        Vector2Int tilePos = TexturePosition(direction, voxelType);

        UVs[0] = new Vector2(
            VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset,
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset
        );

        UVs[1] = new Vector2(
            VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset,
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset
        );

        UVs[2] = new Vector2(
            VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset,
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset
        );
        UVs[3] = new Vector2(
            VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset,
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset
        );

        return UVs;

    }

}
