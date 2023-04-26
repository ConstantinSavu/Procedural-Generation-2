using System.Linq;
using UnityEditor;
using UnityEngine;


public abstract class CustomRenderer : MonoBehaviour {
    public abstract void RenderMesh(MeshData meshData);
}
