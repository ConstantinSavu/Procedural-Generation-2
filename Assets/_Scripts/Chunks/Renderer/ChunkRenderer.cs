using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class ChunkRenderer : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider solidMeshCollider;
    MeshCollider waterMeshCollider;
    Mesh mesh;
    public bool showGizmo = false;

    public ChunkData ChunkData {get; private set;}

    public bool modifiedByPlayer {

        get{
            return ChunkData.modifiedByPlayer;
        }
        set{
            ChunkData.modifiedByPlayer = value;
        }

    }

    private void Awake(){

        meshFilter = GetComponent<MeshFilter>();
        solidMeshCollider = GetComponent<MeshCollider>();

        mesh = meshFilter.mesh;

    }

    public void InitialiseChunk(ChunkData data){
        this.ChunkData = data;
    }

    private void RenderMesh(MeshData meshData){
        mesh.Clear();

        mesh.subMeshCount = 2;
        mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray();

        mesh.SetTriangles(meshData.triangles.ToArray(), 0);
        mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1);

        mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray();
        mesh.RecalculateNormals();

        solidMeshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.colliderVertices.ToArray();
        collisionMesh.triangles = meshData.colliderTriangles.ToArray();
        collisionMesh.RecalculateNormals();

        solidMeshCollider.sharedMesh = collisionMesh;

    }

    public void UpdateChunk(){
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

    public void UpdateChunk(MeshData data){
        RenderMesh(data);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos(){
        
        if(!showGizmo){
            return;
        }

        if(!Application.isPlaying){
            return;
        }

        if(Selection.activeObject == gameObject){
            Gizmos.color = new Color(0, 1, 0, 0.4f);
        }
        else{
            Gizmos.color = new Color(1, 0, 1, 0.4f);
        }

        Gizmos.DrawCube(transform.position + new Vector3(
                ChunkData.chunkSize.x / (float)2 - 0.5f,
                ChunkData.chunkSize.y / (float)2 - 0.5f,
                ChunkData.chunkSize.z / (float)2 - 0.5f),
            ChunkData.chunkSize);

    }

#endif

}
