using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] GameObject RenderedChunks;
    [SerializeField] NavMeshSurface surface;

    public void SetupNavMeshManager(GameObject RenderedChunks){
        this.RenderedChunks = RenderedChunks;
        CreateNavMeshes(RenderedChunks);
    }

    void Update()
    {
        if(RenderedChunks != null && Input.GetKeyDown(KeyCode.B)){
            CreateNavMeshes(RenderedChunks);
        }
    }

    private void CreateNavMeshes(GameObject RenderedChunks)
    {
        
        var watch = System.Diagnostics.Stopwatch.StartNew();
        watch.Start();

        surface = RenderedChunks.GetComponent<NavMeshSurface>();

        surface.BuildNavMesh();
        
        
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("NavMesh " + elapsedMs);
    }

    public void UpdateNavMesh(){
        //surface.UpdateNavMesh(surface.navMeshData);
    }
}
