
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderManager : MonoBehaviour
{
    [SerializeField] GameObject borderPrefab;

    public void SetupBorder(World world){

        Vector3 voxelSize = world.worldSettings.voxelSize;

        Vector3Int voxelStart = - Vector3Int.one + (world.worldData.worldSettings.chunkDrawingRange + Vector3Int.one) * world.worldData.worldSettings.chunkSize;
        Vector3Int voxelEnd = - (world.worldData.worldSettings.chunkDrawingRange) * world.worldData.worldSettings.chunkSize;
        
        Vector3 start = Vector3.Scale(voxelStart, voxelSize);
        Vector3 end = Vector3.Scale(voxelEnd, voxelSize);

        Vector3 scale = 3 * (world.worldSettings.chunkDrawingRange + Vector3.one);

        scale = Vector3.Scale(scale, world.worldSettings.chunkSize);

        scale = Vector3.Scale(scale, voxelSize);

        GameObject startXBorder = Instantiate(borderPrefab, transform.position, transform.rotation, transform);

        startXBorder.transform.position = new Vector3(start.x + voxelSize.x / 2, 0 , 0);
        startXBorder.transform.localScale = new Vector3(0.01f, scale.y, scale.z);

        GameObject endXBorder = Instantiate(borderPrefab, transform.position, transform.rotation, transform);

        endXBorder.transform.position = new Vector3(end.x - voxelSize.x / 2, 0 , 0);
        endXBorder.transform.localScale = new Vector3(0.01f, scale.y, scale.z);

        GameObject startZborder = Instantiate(borderPrefab, transform.position, transform.rotation, transform);

        startZborder.transform.position = new Vector3(0, 0 , start.z + voxelSize.z / 2);
        startZborder.transform.localScale = new Vector3(scale.x, scale.y, 0.01f);


        GameObject endZborder = Instantiate(borderPrefab, transform.position, transform.rotation, transform);

        endZborder.transform.position = new Vector3(0, 0 , end.z - voxelSize.z / 2);
        endZborder.transform.localScale = new Vector3(scale.x, scale.y, 0.01f);
        

    }
}
