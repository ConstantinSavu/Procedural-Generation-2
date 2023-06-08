using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderManager : MonoBehaviour
{
    [SerializeField] GameObject borderPrefab;

    public void SetupBorder(World world){


        Vector3 scale = 2 * world.worldSettings.chunkDrawingRange;

        scale = Vector3.Scale(scale, world.worldSettings.chunkSize);

        scale = Vector3.Scale(scale, world.worldSettings.voxelSize);

        scale.y = 1;

        GameObject northBorder = Instantiate(borderPrefab, transform.position, transform.rotation, transform);

        
        northBorder.transform.Rotate(Vector3.forward * 90);
        northBorder.transform.position = Vector3.right * (scale.x/2 + world.worldSettings.chunkSize.x * world.worldSettings.voxelSize.x);
        scale.y = 0.1f;
        northBorder.transform.localScale = scale * 10;
    

        GameObject southBorder = Instantiate(borderPrefab, transform.position, transform.rotation, transform);

        
        southBorder.transform.Rotate(Vector3.forward * 90);
        southBorder.transform.position = -Vector3.right * (scale.x/2);
        scale.y = 0.1f;
        southBorder.transform.localScale = scale * 10;


        GameObject eastBorder = Instantiate(borderPrefab, transform.position, transform.rotation, transform);

        
        eastBorder.transform.Rotate(Vector3.right * 90);
        eastBorder.transform.position = Vector3.forward * (scale.z/2 - 2 * world.worldSettings.chunkSize.z * world.worldSettings.voxelSize.z);
        scale.y = 0.1f;
        eastBorder.transform.localScale = scale * 10;

        GameObject westBorder = Instantiate(borderPrefab, transform.position, transform.rotation, transform);

        
        westBorder.transform.Rotate(Vector3.right * 90);
        westBorder.transform.position = -Vector3.forward * (scale.z/2 + 3 * world.worldSettings.chunkSize.z * world.worldSettings.voxelSize.z);
        scale.y = 0.1f;
        westBorder.transform.localScale = scale * 10;

        
        

    }
}
