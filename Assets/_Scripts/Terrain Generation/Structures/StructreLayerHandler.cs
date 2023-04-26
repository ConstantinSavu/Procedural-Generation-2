using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StructreLayerHandler : MonoBehaviour
{
    
    [SerializeField]

    private StructreLayerHandler Next;

    public bool Handle(ChunkData data, Vector3Int pos, BiomeGenerator biomeGenerator){

        if(TryHandling(data, pos, biomeGenerator)){
            return true;
        }
        
        if(Next != null){
            return Next.Handle(data, pos, biomeGenerator);
        }

        return false;
    }

    protected abstract bool TryHandling(ChunkData data, Vector3Int pos, BiomeGenerator biomeGenerator);

}
