using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] GameObject RenderedChunks;
    [SerializeField] NavMeshSurface[] surfaces;

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
        
        surfaces = RenderedChunks.GetComponentsInChildren<NavMeshSurface>();

        foreach(NavMeshSurface surface in surfaces){
            surface.RemoveData();
            surface.BuildNavMesh();
            
            surface.AddData();
        }
        
        
        
        

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("NavMesh " + elapsedMs);
    }
}
