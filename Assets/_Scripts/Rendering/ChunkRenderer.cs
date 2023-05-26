
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    public CustomRenderer solidRenderer;
    public CustomRenderer liquidRenderer;

    public bool showGizmo = false;

    [SerializeField]
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

    }

    public void InitialiseChunk(ChunkData data){
        this.ChunkData = data;
    }

    private void RenderMesh(MeshData meshData){

        solidRenderer.RenderMesh(meshData);
        liquidRenderer.RenderMesh(meshData);

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
