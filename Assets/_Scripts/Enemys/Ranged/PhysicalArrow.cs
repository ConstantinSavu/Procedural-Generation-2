using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalArrow : Projectile
{
    [SerializeField] public float timeToLiveAfterPlayerHit = 1f;
    [SerializeField] public int damageValue = 1;
    public Transform target;

    Coroutine timeToLiveCoRoutine;

    void Awake()
    {
        StartTimer(timeToLive, DestroyObject, ref timeToLiveCoRoutine);

        if(rigidBody == null){
            rigidBody = transform.GetComponent<Rigidbody>();
        }

        if(collider == null){
            collider = transform.GetComponent<CapsuleCollider>();
        }        
        
    }
   
    void OnCollisionEnter(Collision other)
    {
        
        HandleEnterCollider(other.collider);
        
    }

    void OnTriggerEnter(Collider other)
    {
        HandleEnterCollider(other);
    }

    private new void HandleEnterCollider(Collider other){

        rigidBody.drag = normalDrag;

        if(waterMask == -1){
            waterMask = 1 << LayerMask.NameToLayer("Water");
        }
        
        if(1 << other.gameObject.layer == waterMask){
            Debug.Log(transform.name + " hit water");
            rigidBody.drag = waterDrag;
            rigidBody.excludeLayers = waterMask;
            collider.excludeLayers = waterMask;
            return;
        }

        if(hitpoints < 1){
            return;
        }

        hitpoints--;

        Debug.Log("Arrow hit: " + other.gameObject.name);
        transform.parent = other.gameObject.transform;
        
        
        rigidBody.AddForce(Vector3.zero, ForceMode.VelocityChange);
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        rigidBody.detectCollisions = false;

        
        if(collider != null && collider.enabled == true){
            collider.enabled = false;
        }

        LayerMask layerMask = 1 << other.gameObject.layer;
        
        if((layerMask | damageMask) == damageMask){
            HealthSystem playerHealthSystem;
            if(other.transform.TryGetComponent<HealthSystem>(out playerHealthSystem)){
                Debug.Log(transform.name + " damaged " + other.transform.name);
                playerHealthSystem.TakeDamage(damageValue);
                StartTimer(timeToLiveAfterPlayerHit, DestroyObject, ref timeToLiveCoRoutine);

            }
        }

    }

    

    void DestroyObject(){
        Destroy(transform.gameObject);
    }

    

}
