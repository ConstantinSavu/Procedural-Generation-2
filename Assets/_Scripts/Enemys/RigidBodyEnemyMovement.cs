using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyEnemyMovement : MonoBehaviour
{
    public Transform target;
    public float stoppingDistance = 0.1f;
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;

    private Rigidbody rb;

    public float angle;

    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 targetDirection = target.position - transform.position;
        rb.MoveRotation(target.rotation);

        float distance = Vector3.Distance(transform.position, target.position);

        if(distance < stoppingDistance){
            return;
        }

        targetDirection.Normalize();

        rb.MovePosition(transform.position + (targetDirection * movementSpeed * Time.deltaTime));
        
        
        
    }
}
