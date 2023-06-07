using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float timeToLive = 100f;
    [SerializeField] protected Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] protected Vector3 speed = new Vector3(5f, 5f, 5f);
    [SerializeField] Vector3 maxSpeed = new Vector3(20f, 20f, 20f);
    
    [SerializeField] [Range(0, -180f)]
    float minRandomAngle = -5f;

    [SerializeField] [Range(0, 180f)]
    float maxRandomAngle = 5f;
    [SerializeField] protected float waterDrag = 20f;
    [SerializeField] protected float normalDrag = 0f;
    [SerializeField] int solverIterations;

    protected Rigidbody rigidBody;
    protected new Collider collider;
    protected int hitpoints = 1;
    [SerializeField] protected LayerMask waterMask;
    [SerializeField] protected LayerMask damageMask;

    void OnEnable()
    {
        waterMask = 1 << LayerMask.NameToLayer("Water");
        LayerMask playerMask = 1 << LayerMask.NameToLayer("Player");
        LayerMask enemyMask = 1 << LayerMask.NameToLayer("Enemy");

        damageMask = playerMask | enemyMask;
    }

    void OnCollisionEnter(Collision other)
    {
        
        HandleEnterCollider(other.collider);
        
    }

    void OnTriggerEnter(Collider other)
    {
        HandleEnterCollider(other);
    }

    protected void HandleEnterCollider(Collider other){

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

        Debug.Log(transform.name + " hit: " + other.gameObject.name);
        transform.parent = other.gameObject.transform;
        
        
        rigidBody.AddForce(Vector3.zero, ForceMode.VelocityChange);
        DisableRigidBody();
        
        
    }
    
    public void DisableRigidBody(){
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        collider.isTrigger = false;
    }

    public void EnableRigidBody(){
        hitpoints++;
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        collider.isTrigger = true;
    }

    public void Shoot(Vector3 relativeSpeed){

        transform.localScale = scale;
       
        Vector3 resultatSpeed = speed.x * transform.right +   
                                speed.y * transform.up +      
                                speed.z * transform.forward;

        resultatSpeed += relativeSpeed;

        rigidBody.AddForce(resultatSpeed, ForceMode.VelocityChange);
    }

    public void Shoot(){

        transform.localScale = scale;
       
        Vector3 resultatSpeed = speed.x * transform.right +   
                                speed.y * transform.up +      
                                speed.z * transform.forward;

        rigidBody.AddForce(resultatSpeed, ForceMode.VelocityChange);
    }

    void Shoot(Transform target){
        
        transform.localScale = scale;

        Vector3 difference = 
        Vector3Abs(
            transform.position - 
            target.position
        );
        
        Vector3 randomSpread = new Vector3(
        UnityEngine.Random.Range(minRandomAngle, maxRandomAngle),
        UnityEngine.Random.Range(minRandomAngle, maxRandomAngle),
        UnityEngine.Random.Range(minRandomAngle, maxRandomAngle));
        
        if(randomSpread != Vector3.zero){
            transform.Rotate(randomSpread);
        }
        
        Vector3 localArrowSpeed = Vector3.Scale(difference, speed);

        Vector3 resultantSpeed = Vector3Clamp(localArrowSpeed, Vector3.zero, maxSpeed);
       
        Vector3 resultatSpeed = resultantSpeed.x * transform.right +   
                                resultantSpeed.y * transform.up +      
                                resultantSpeed.z * transform.forward;

        if(rigidBody == null){
            rigidBody = transform.GetComponent<Rigidbody>();
        }

        if(collider == null){
            collider = transform.GetComponent<CapsuleCollider>();
        }
        
        rigidBody.AddForce(resultatSpeed, ForceMode.VelocityChange);
    }

    protected void StartTimer(float seconds, Action callback, ref Coroutine coroutine){
        
        if(coroutine != null){
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(Timer(seconds, callback));
    }

    protected IEnumerator Timer(float seconds, Action callback){
        yield return new WaitForSeconds(seconds);

        callback();
    }

    void OnValidate()
    {
        if(rigidBody != null){
            solverIterations = solverIterations > 0 ? solverIterations : 0;

            rigidBody.solverIterations = solverIterations;
        }
        
    }

    private Vector3 Vector3Abs(Vector3 vector){
        return new Vector3(
            Mathf.Abs(vector.x),
            Mathf.Abs(vector.y),
            Mathf.Abs(vector.z)
        );
    }

    private Vector3 Vector3Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        return new Vector3(
            Mathf.Clamp(value.x, min.x , max.x),
            Mathf.Clamp(value.y, min.y , max.y),
            Mathf.Clamp(value.z, min.z , max.z)
        );
    }
}
