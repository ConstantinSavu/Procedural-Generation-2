using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    
    public BiomeGenerator biomeGenerator;
    public CaveGenerator caveGenerator;

    public ChunkData GenerateChunkData(ChunkData data){
        
        Parallel.For(0, data.chunkSize.x, x => {
            for(int z = 0; z < data.chunkSize.z; z++){
                int groundPosition = biomeGenerator.Get2DTerrainY(x + data.worldPosition.x, z + data.worldPosition.z, data);
                for(int y = 0; y < data.chunkSize.y; y++){
                    data = biomeGenerator.ProcessVoxel(data, new Vector3Int(x, y, z), groundPosition);
                }
            }

        });

        data = biomeGenerator.ProcessStructures(data);
        

        return data;

    }

}
