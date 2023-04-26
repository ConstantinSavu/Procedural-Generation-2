using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class SolidRenderer : MonoBehaviour{

   MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;

    private void Awake(){

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        mesh = meshFilter.mesh;

    }

    public void RenderMesh(MeshData meshData){
        mesh.Clear();

        mesh.subMeshCount = 1;
        mesh.vertices = meshData.vertices.ToArray();

        mesh.SetTriangles(meshData.triangles.ToArray(), 0);

        mesh.uv = meshData.uv.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.colliderVertices.ToArray();
        collisionMesh.triangles = meshData.colliderTriangles.ToArray();
        collisionMesh.RecalculateNormals();

        meshCollider.sharedMesh = collisionMesh;

    }


}
