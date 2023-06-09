
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalFireBall : MonoBehaviour
{
    [SerializeField] public float timeToLiveInSeconds = 10f;
    [SerializeField] public float timeToLiveAfterPlayerHitInSeconds = 1f;
    [SerializeField] public float timeAfterHit = 2f;
    [SerializeField] public int damageValue = 1;
    [SerializeField] public Vector3 fireBallScale =  new Vector3(0.5f, 0.5f, 0.5f);

    [SerializeField] [Range(0, -180f)]
    float minRandomAngle = -5f;

    [SerializeField] [Range(0, 180f)]
    float maxRandomAngle = 5f;

    [SerializeField] Vector3 globalFireBallSpeed = new Vector3(20f, 20f, 20f);
    [SerializeField] Vector3 maxFireBallSpeed = new Vector3(20f, 20f, 20f);

    private int hitpoints = 1;
    Rigidbody rigidBody;
    CapsuleCollider capsuleColider;

    [SerializeField] LayerMask waterMask;
    [SerializeField] LayerMask playerMask;

    [SerializeField] LayerMask enemyMask;

    public Transform target;
    private bool wasShot = false;

    [SerializeField] GameObject fireOrb;

    [SerializeField] GameObject destroySmoke;

    void Awake()
    {
        StopCoroutine(TimeToLive(timeToLiveInSeconds));
        StartCoroutine(TimeToLive(timeToLiveInSeconds));

        if(rigidBody == null){
            rigidBody = transform.GetComponent<Rigidbody>();
        }
        if(capsuleColider == null){
            capsuleColider = transform.GetComponent<CapsuleCollider>();
        }

        waterMask = 1 << LayerMask.NameToLayer("Water");
        playerMask = 1 << LayerMask.NameToLayer("Player");
        enemyMask = 1 << LayerMask.NameToLayer("Enemy");

        
        
    }
   
    
    void Start()
    {
        if(target != null && !wasShot){
            wasShot = true;
            Shoot(target);
        }
    }

    void Update()
    {   
        if(target != null && !wasShot){
            wasShot = true;
            Shoot(target);
        }
    }

    void Shoot(Transform target){
        
        transform.localScale = fireBallScale;

        Vector3 difference = 
        Vector3Abs(
            transform.position - 
            target.position
        );

        Vector3 localArrowSpeed = Vector3.Scale(difference, globalFireBallSpeed);

        Vector3 resultantSpeed = Vector3Clamp(localArrowSpeed, Vector3.zero, maxFireBallSpeed);
       
        Vector3 resultatSpeed = resultantSpeed.x * transform.right +   
                                resultantSpeed.y * transform.up +      
                                resultantSpeed.z * transform.forward;
         
        
        Vector3 randomSpread = new Vector3(
        Random.Range(minRandomAngle, maxRandomAngle), 
        Random.Range(minRandomAngle, maxRandomAngle), 
        Random.Range(minRandomAngle, maxRandomAngle));
        
        if(randomSpread != Vector3.zero){
            transform.Rotate(randomSpread);
        }
        

        if(rigidBody == null){
            rigidBody = transform.GetComponent<Rigidbody>();
        }

        if(capsuleColider == null){
            capsuleColider = transform.GetComponent<CapsuleCollider>();
        }
        
        rigidBody.AddForce(resultatSpeed, ForceMode.VelocityChange);
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

        Debug.Log("Fireball hit: " + other.gameObject.name);
        transform.parent = other.gameObject.transform;
        
        
        rigidBody.AddForce(Vector3.zero, ForceMode.VelocityChange);
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        rigidBody.detectCollisions = false;

        
        if(capsuleColider != null && capsuleColider.enabled == true){
            capsuleColider.enabled = false;
        }

        
        if(1 << other.gameObject.layer == playerMask){
            HealthSystem playerHealthSystem;
            if(other.transform.TryGetComponent<HealthSystem>(out playerHealthSystem)){
                Debug.Log("Damaged Player ");
                playerHealthSystem.TakeDamage(damageValue);

                StartDestroy(timeToLiveAfterPlayerHitInSeconds);

            }

            return;
        }

        if(1 << other.gameObject.layer == enemyMask){
            
            EnemyHealthSystem enemyHealthSystem;
            Debug.Log("Damaged Enemy: " + other.gameObject.name);
            
            if(other.transform.TryGetComponent<EnemyHealthSystem>(out enemyHealthSystem)){
                enemyHealthSystem.TakeDamage(damageValue);
            }

            StartDestroy(timeToLiveAfterPlayerHitInSeconds);
            

            return;
        }

        
        StartDestroy(timeAfterHit);
        
        
    }

    private void StartDestroy(float timeToLive){

        fireOrb.SetActive(false);
        destroySmoke.SetActive(true);
        
        StopCoroutine(TimeToLive(timeToLive));
        StartCoroutine(TimeToLive(timeToLive));
            
    }

    private IEnumerator TimeToLive(float timeToLiveInSeconds){
        yield return new WaitForSeconds(timeToLiveInSeconds);

        Destroy(transform.gameObject);
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
