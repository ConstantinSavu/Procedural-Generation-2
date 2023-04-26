using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class StructureGenerator : MonoBehaviour
{
    public StructreLayerHandler firstStructureLayerHandler;
    public List<StructreLayerHandler> additionalStructureLayerHandlers;
    
    public abstract List<Vector3Int> GenerateStructureData(ChunkData chunkData);
    
} 
