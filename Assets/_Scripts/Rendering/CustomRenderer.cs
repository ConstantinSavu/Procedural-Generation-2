using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CustomRenderer : MonoBehaviour {

    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;
    protected Mesh mesh;
    protected void Awake()
    {
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
        
        if(meshData.vertices.Count > 0){
            meshCollider.sharedMesh = null;
            Mesh collisionMesh = new Mesh();
            collisionMesh.vertices = meshData.vertices.ToArray();
            collisionMesh.triangles = meshData.triangles.ToArray();
            collisionMesh.RecalculateNormals();

            meshCollider.sharedMesh = collisionMesh;
        }
        

        
    }

    private void OnEnable()
    {
        Physics.BakeMesh(mesh.GetInstanceID(), false);
    }

}
