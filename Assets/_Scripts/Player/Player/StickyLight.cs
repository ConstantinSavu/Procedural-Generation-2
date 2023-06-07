using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Light))]
[RequireComponent(typeof(MeshRenderer))]

public class StickyLight : Projectile
{

    
    Rigidbody rigidBody;
    SphereCollider sphereCollider;
    Light pointLight;

    MeshRenderer meshRenderer;

    [SerializeField] float timeToLive = 100f;
    Coroutine destroyCoroutine;
    [SerializeField] bool canDie = false;
    [SerializeField] Color currentColor;
    [SerializeField] Color nextColor;
    [SerializeField] float timeToNextColor = 1f;
    Coroutine pickNextColorCoroutine;

    private int hitpoints = 1;
    
    
    void Awake()
    {
        
    }
    
    void Start()
    {
        if(rigidBody == null){
            rigidBody = transform.GetComponent<Rigidbody>();
        }

        if(sphereCollider == null){
            sphereCollider = transform.GetComponent<SphereCollider>();
        }

        sphereCollider.isTrigger = true;

        if(pointLight == null){
            pointLight = transform.GetComponent<Light>();
        }

        if(meshRenderer == null){
            meshRenderer = transform.GetComponent<MeshRenderer>();
        }

        StartDestroy(timeToLive);

        currentColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        nextColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        pointLight.color = currentColor;
        meshRenderer.material.color = currentColor;

        StartPickNextColor(timeToNextColor);
    }

    // Update is called once per frame
    void Update()
    {

        currentColor = Color.Lerp(currentColor, nextColor, Mathf.PingPong(Time.time, 1));

        pointLight.color = currentColor;
        meshRenderer.material.color = currentColor;

        if(canDie){
            pointLight.intensity -= 0.01f;
        }

        if(pointLight.intensity <= 0){
            Destroy(transform.gameObject);
        }

    }

     void OnCollisionEnter(Collision other)
    {
        
        HandleCollider(other.collider);
        
    }

    void OnTriggerEnter(Collider other)
    {
        HandleCollider(other);
    }

    private void HandleCollider(Collider other){

        if(hitpoints < 1){
            return;
        }

        hitpoints--;

        Debug.Log("StickyLight hit: " + other.gameObject.name);
        transform.parent = other.gameObject.transform;
        
        
        rigidBody.AddForce(Vector3.zero, ForceMode.VelocityChange);
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        rigidBody.detectCollisions = false;

        
        if(sphereCollider != null && sphereCollider.enabled == true){
            sphereCollider.enabled = false;
        }
        
    }

    void Shoot(Vector3 speed){
       
        Vector3 resultatSpeed = speed.x * transform.right +   
                                speed.y * transform.up +      
                                speed.z * transform.forward;

        rigidBody.AddForce(resultatSpeed, ForceMode.VelocityChange);
    }

    private void StartDestroy(float timeToLive){
        
        if(destroyCoroutine != null){
            StopCoroutine(destroyCoroutine);
        }

        destroyCoroutine = StartCoroutine(Timer(timeToLive, SetCanDieTrue));
            
    }

    private void StartPickNextColor(float timeToNextColor){
        if(pickNextColorCoroutine != null){
            StopCoroutine(pickNextColorCoroutine);

        }
        pickNextColorCoroutine = StartCoroutine(Timer(timeToNextColor, PickNextColor));
            
    }

    private void SetCanDieTrue(){
        canDie = true;
    }

    private void PickNextColor(){
        nextColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        StartPickNextColor(timeToNextColor);
    }

    private IEnumerator Timer(float seconds, Action callback){
        yield return new WaitForSeconds(seconds);

        callback();
    }

}
