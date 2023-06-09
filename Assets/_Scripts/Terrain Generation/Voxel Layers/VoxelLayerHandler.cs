using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoxelLayerHandler : MonoBehaviour
{
    
    [SerializeField]

    private VoxelLayerHandler Next;


    [SerializeField] protected List<VoxelType> canReplace = new List<VoxelType>();
    [SerializeField] protected List<VoxelType> cannotReplace = new List<VoxelType>();
    
    protected HashSet<VoxelType> canReplaceHashSet;
    protected HashSet<VoxelType> cannotReplaceHashSet;

    void Awake()
    {
        canReplaceHashSet = new HashSet<VoxelType>(canReplace);
        cannotReplaceHashSet = new HashSet<VoxelType>(cannotReplace);
    }

    public bool Handle(ChunkData data, Vector3Int pos, int surfaceHeight, ref VoxelType currentVoxel){

        if(TryHandling(data, pos, surfaceHeight, ref currentVoxel)){
            return true;
        }
        
        if(Next != null){
            return Next.Handle(data, pos, surfaceHeight, ref currentVoxel);
        }

        return false;
    }

    protected abstract bool TryHandling(ChunkData data, Vector3Int pos, int surfaceHeight, ref VoxelType currentVoxel);


    void OnValidate()
    {
        canReplaceHashSet = new HashSet<VoxelType>(canReplace);
        cannotReplaceHashSet = new HashSet<VoxelType>(cannotReplace);
    }
}
