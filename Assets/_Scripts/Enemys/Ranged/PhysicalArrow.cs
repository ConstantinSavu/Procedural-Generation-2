using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalArrow : MonoBehaviour
{
    public float timeToLiveInSeconds = 10f;

    private int hitpoints = 1;

    void Awake()
    {
        StopCoroutine(TimeToLive(timeToLiveInSeconds));
        StartCoroutine(TimeToLive(timeToLiveInSeconds));
    }

    void OnCollisionEnter(Collision other)
    {
        if(hitpoints < 1){
            return;
        }

        hitpoints--;

        Debug.Log("Arrow hit: " + other.gameObject.name);
        transform.parent = other.gameObject.transform;
        Rigidbody rb = transform.GetComponent<Rigidbody>();
        
        rb.AddForce(Vector3.zero, ForceMode.VelocityChange);
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;

        CapsuleCollider cc = transform.GetComponent<CapsuleCollider>();
        if(cc != null && cc.enabled == true){
            cc.enabled = false;
        }
        

    }

    private IEnumerator TimeToLive(float timeToLiveInSeconds){
        yield return new WaitForSeconds(timeToLiveInSeconds);

        Destroy(transform.gameObject);
    }
}
