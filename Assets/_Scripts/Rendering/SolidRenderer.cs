using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class SolidRenderer : CustomRenderer
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;

    public GameObject waterObstacles;
    private void Awake(){

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        mesh = meshFilter.mesh;

    }
    public override void RenderMesh(MeshData meshData){
        mesh.Clear();

        mesh.subMeshCount = 1;
        mesh.vertices = meshData.vertices.ToArray();

        mesh.SetTriangles(meshData.triangles.ToArray(), 0);

        mesh.uv = meshData.uv.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.vertices.ToArray();
        collisionMesh.triangles = meshData.triangles.ToArray();
        collisionMesh.RecalculateNormals();

        meshCollider.sharedMesh = collisionMesh;

        /*
        foreach(Vector3 pos in meshData.waterMesh.navMeshObstaclesPositions){
            
            NavMeshModifierVolume addedVolume = waterObstacles.AddComponent<NavMeshModifierVolume>();

            //Set to not walkable
            addedVolume.area = 1;
            addedVolume.center = pos;
            
            addedVolume.size = 1.1f * Vector3.one;
            
        }
        */

    }
}
