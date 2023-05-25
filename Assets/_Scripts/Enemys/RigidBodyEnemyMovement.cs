using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RigidBodyEnemyMovement : MonoBehaviour
{
    public Transform positionTarget;
    public Transform rotationTarget;
    public float stoppingDistance = 0.1f;
    public Vector3 movementSpeed = new Vector3(5f, 5f, 5f);
    public float rotationSpeed = 5f;

    private Rigidbody rb;

    public bool narutoRun = false;

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

        Vector3 targetDirection = positionTarget.position - transform.position;
        

        float distance = Vector3.Distance(transform.position, positionTarget.position);

        if(rotationTarget.rotation != null){
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (rotationTarget.position - transform.position), rotationSpeed * Time.fixedDeltaTime);
            
            if(!narutoRun){
                targetRotation.x = 0;
                targetRotation.z = 0;
            }
            
            
            targetRotation.Normalize();
            rb.MoveRotation(targetRotation);
        }
        else{
            rb.MoveRotation(positionTarget.rotation);
        }

        if(distance < stoppingDistance){
            return;
        }

        targetDirection.Normalize();

        rb.MovePosition(transform.position + (Vector3.Scale(targetDirection, movementSpeed) * Time.fixedDeltaTime));
        
        
        
        
    }
}
