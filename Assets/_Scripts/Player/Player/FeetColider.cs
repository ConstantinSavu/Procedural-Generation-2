using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetColider : MonoBehaviour
{
    public float rayDistance = 0.01f;

    private void OnDrawGizmos(){
        Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
    }

}
