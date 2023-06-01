using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    public MeshData solidMesh;
    public MeshData liquidMesh;
    public MeshData underLiquidMesh;
    public MeshData transparentMesh;
    private bool isMainMesh = true;

    public MeshData(bool isMainMesh){

        if(isMainMesh){

            solidMesh = new MeshData(false);
            liquidMesh = new MeshData(false);
            underLiquidMesh = new MeshData(false);
            transparentMesh = new MeshData(false);

        }

    }

    public void AddVertex(Vector3 vertex){
        vertices.Add(vertex);
    }

    public void AddQuadTriangles(bool triangleAddCollider){

        int verticesCount = vertices.Count;

        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);

        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);

    }

}
