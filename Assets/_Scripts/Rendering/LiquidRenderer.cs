using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class LiquidRenderer : CustomRenderer
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
        mesh.vertices = meshData.waterMesh.vertices.ToArray();

        mesh.SetTriangles(meshData.waterMesh.triangles.ToArray(), 0);

        mesh.uv = meshData.waterMesh.uv.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.waterMesh.vertices.ToArray();
        collisionMesh.triangles = meshData.waterMesh.triangles.ToArray();
        collisionMesh.RecalculateNormals();

        meshCollider.sharedMesh = collisionMesh;

        foreach(Vector3 pos in meshData.waterMesh.navMeshObstaclesPositions){
            
            GameObject waterObstacle = new GameObject(pos.ToString());
            NavMeshObstacle addedVolume = waterObstacle.AddComponent<NavMeshObstacle>();

            //Set to not walkable
            addedVolume.shape = NavMeshObstacleShape.Box;
            addedVolume.size = 1.1f * Vector3.one;
            waterObstacle.transform.SetParent(waterObstacles.transform);
            waterObstacle.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);
            
        }

    }
}
