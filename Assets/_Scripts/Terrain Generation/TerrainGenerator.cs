using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    
    public BiomeGenerator biomeGenerator;   

    public ChunkData GenerateChunkData(ChunkData data){
        
        int voxelCount = data.chunkSize.x * data.chunkSize.y * data.chunkSize.z;


        Parallel.For(0, voxelCount, index => {

            int x = index % data.chunkSize.x;
            int y = (index / data.chunkSize.x) % data.chunkSize.y;
            int z = index / (data.chunkSize.x * data.chunkSize.y);
            
            int groundPosition;

            groundPosition = biomeGenerator.Get2DTerrainY(x, z, data);
            
            data = biomeGenerator.ProcessVoxel(data, new Vector3Int(x, y, z), groundPosition);
                
            

        });

        data = biomeGenerator.ProcessStructures(data);
        

        return data;

    }

}
