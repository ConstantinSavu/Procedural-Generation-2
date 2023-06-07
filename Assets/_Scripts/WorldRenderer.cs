using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRenderer : MonoBehaviour
{
    public GameObject chunkPrefab;
    public Queue<ChunkRenderer> chunkPool = new Queue<ChunkRenderer>();
    public GameObject RenderedChunks;
    public World worldReference;

    public void Clear(World.WorldData worldData){

        foreach(var item in worldData.chunkDictionary.Values){
            Destroy(item.gameObject);
        }

        chunkPool.Clear();

    }

    public ChunkRenderer InstantiateChunk(Vector3 pos){
        
        ChunkRenderer newChunk;
        Vector3 actualPos = Vector3.Scale(pos, worldReference.worldSettings.voxelSize);
        GameObject chunkObject = Instantiate(chunkPrefab, actualPos, Quaternion.identity, RenderedChunks.transform);
        newChunk = chunkObject.GetComponent<ChunkRenderer>();
        chunkPool.Enqueue(newChunk);
        
        return newChunk;
    }

    public ChunkRenderer RenderChunk(ChunkData chunkData, Vector3Int pos, MeshData meshData){

        ChunkRenderer newChunk = null;

        if(chunkPool.Count > 0){
            newChunk = chunkPool.Dequeue();
            newChunk.transform.position = Vector3.Scale(pos, chunkData.worldReference.worldSettings.voxelSize);
        }
        else{
            Debug.Log("We no so smort");
        }
        
        newChunk.InitialiseChunk(chunkData);
        newChunk.UpdateChunk(meshData);
        newChunk.gameObject.SetActive(true);
        newChunk.gameObject.name = pos.ToString();

        return newChunk;
    }

    public void RemoveChunk(ChunkRenderer chunk){
        chunk.gameObject.SetActive(false);
        chunkPool.Enqueue(chunk);
    }
}
