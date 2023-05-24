using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public HashSet<Vector3> navMeshObstaclesPositions = new HashSet<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    public MeshData waterMesh;
    private bool isMainMesh = true;

    public MeshData(bool isMainMesh){

        if(isMainMesh){

            waterMesh = new MeshData(false);

        }

    }

    public void AddVertex(Vector3 vertex, bool vertexAddCollider){
        
        vertices.Add(vertex);


    }

    public void AddQuadTriangles(bool triangleAddCollider, bool isLiquid){

        int verticesCount = vertices.Count;

        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);

        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);

    }

}
