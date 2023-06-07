using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Light))]
[RequireComponent(typeof(MeshRenderer))]

public class StickyLight : Projectile
{
    Light pointLight;
    MeshRenderer meshRenderer;
    Coroutine destroyCoroutine;
    [SerializeField] bool canDie = false;
    [SerializeField] Color currentColor;
    [SerializeField] Color nextColor;
    [SerializeField] float timeToNextColor = 1f;
    [SerializeField] private float damping = 1f;
    Coroutine pickNextColorCoroutine;
    

    void Awake()
    {
        if(collider == null){
            collider = transform.GetComponent<SphereCollider>();
            collider.isTrigger = true;
        }

        if(rigidBody == null){
            rigidBody = transform.GetComponent<Rigidbody>();
        }

        if(pointLight == null){
            pointLight = transform.GetComponent<Light>();
        }

        if(meshRenderer == null){
            meshRenderer = transform.GetComponent<MeshRenderer>();
        }

    }
    
    void Start()
    {
        StartTimer(timeToLive, SetCanDieTrue, ref destroyCoroutine);

        currentColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        nextColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        pointLight.color = currentColor;
        meshRenderer.material.color = currentColor;

        StartPickNextColor(timeToNextColor);
    }

    // Update is called once per frame
    void Update()
    {

        currentColor = Color.Lerp(currentColor, nextColor, Time.deltaTime * damping);

        pointLight.color = currentColor;
        meshRenderer.material.color = currentColor;

        if(canDie){
            pointLight.intensity -= 0.01f;
        }

        if(pointLight.intensity <= 0){
            Destroy(transform.gameObject);
        }

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

    

}
