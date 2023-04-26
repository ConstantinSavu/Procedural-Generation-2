using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoxelLayerHandler : MonoBehaviour
{
    
    [SerializeField]

    private VoxelLayerHandler Next;

    public bool Handle(ChunkData data, Vector3Int pos, int surfaceHeight){

        if(TryHandling(data, pos, surfaceHeight)){
            return true;
        }
        
        if(Next != null){
            return Next.Handle(data, pos, surfaceHeight);
        }

        return false;
    }

    protected abstract bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight);

}
