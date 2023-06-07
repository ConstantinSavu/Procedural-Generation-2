using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalArrow : MonoBehaviour
{
    [SerializeField] public float timeToLiveInSeconds = 10f;
    [SerializeField] public float timeToLiveAfterPlayerHitInSeconds = 1f;
    [SerializeField] public float damageValue = 1f;
    [SerializeField] public Vector3 arrowScale =  new Vector3(0.5f, 0.5f, 0.5f);

    [SerializeField] [Range(0, -180f)]
    float minRandomAngle = -5f;

    [SerializeField] [Range(0, 180f)]
    float maxRandomAngle = 5f;

    [SerializeField] Vector3 globalArrowSpeed = new Vector3(20f, 20f, 20f);
    [SerializeField] Vector3 maxArrowSpeed = new Vector3(20f, 20f, 20f);

    private int hitpoints = 1;

    [SerializeField] bool inWater = false;

    Rigidbody rigidBody;
    CapsuleCollider capsuleColider;

    [SerializeField] Vector3 waterBuoyancy = - Physics.gravity / 1.2f;
    [SerializeField] float waterEnterDampen = 10f;

    [SerializeField] LayerMask waterMask;
    [SerializeField] LayerMask playerMask;

    [SerializeField] LayerMask enemyMask;

    public Transform target;
    private bool wasShot = false;

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
   
    void Shoot(Transform target){
        
        this.target = target;
        transform.localScale = arrowScale;

        Vector3 difference = 
        Vector3Abs(
            transform.position - 
            target.position
        );
        
        Vector3 randomSpread = new Vector3(
        Random.Range(minRandomAngle, maxRandomAngle), 
        Random.Range(minRandomAngle, maxRandomAngle), 
        Random.Range(minRandomAngle, maxRandomAngle));
        
        if(randomSpread != Vector3.zero){
            transform.Rotate(randomSpread);
        }
        
        Vector3 localArrowSpeed = Vector3.Scale(difference, globalArrowSpeed);

        Vector3 resultantSpeed = Vector3Clamp(localArrowSpeed, Vector3.zero, maxArrowSpeed);
       
        Vector3 resultatSpeed = resultantSpeed.x * transform.right +   
                                resultantSpeed.y * transform.up +      
                                resultantSpeed.z * transform.forward;

        if(rigidBody == null){
            rigidBody = transform.GetComponent<Rigidbody>();
        }

        if(capsuleColider == null){
            capsuleColider = transform.GetComponent<CapsuleCollider>();
        }
        
        rigidBody.AddForce(resultatSpeed, ForceMode.VelocityChange);
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

    void FixedUpdate()
    {
        
        if(inWater){
            rigidBody.AddForce(waterBuoyancy * rigidBody.mass);
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

        if(1 << other.gameObject.layer == waterMask){
            Debug.Log("Arrow hit water");
            inWater = true;
            rigidBody.excludeLayers = waterMask;
            capsuleColider.excludeLayers = waterMask;
            rigidBody.AddForce(rigidBody.velocity/waterEnterDampen, ForceMode.VelocityChange);
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

        
        if(capsuleColider != null && capsuleColider.enabled == true){
            capsuleColider.enabled = false;
        }
        
        if(1 << other.gameObject.layer == playerMask){
            HealthSystem playerHealthSystem;
            if(other.transform.TryGetComponent<HealthSystem>(out playerHealthSystem)){
                Debug.Log("Damaged Player ");
                playerHealthSystem.TakeDamage(damageValue);

                StopCoroutine(TimeToLive(timeToLiveAfterPlayerHitInSeconds));
                StartCoroutine(TimeToLive(timeToLiveAfterPlayerHitInSeconds));

            }
        }

        if(1 << other.gameObject.layer == enemyMask){
            EnemyHealthSystem enemyHealthSystem;
            Debug.Log("Damaged Enemy: " + other.gameObject.name);
            if(other.transform.TryGetComponent<EnemyHealthSystem>(out enemyHealthSystem)){
                enemyHealthSystem.TakeDamage(damageValue);
            }
        }
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
