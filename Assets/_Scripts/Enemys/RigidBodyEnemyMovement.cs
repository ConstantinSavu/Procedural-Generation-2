using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RigidBodyEnemyMovement : MonoBehaviour
{
    public Transform navMeshTarget;
    public Transform playerTarget;
    private NavMeshAgent navMeshTargetAgent;    
    public float stoppingDistance = 0.1f;
    public float attackRange = 1.5f;
    public Vector3 movementSpeed = new Vector3(5f, 0f, 5f);
    public float rotationSpeed = 5f;

    private Rigidbody rb;

    public bool narutoRun = false;

    Animator animator;

    [SerializeField] float health = 3;
    private bool isAttacking = false;
    public float animationFinnishTime = 0.9f;
    [SerializeField] private float jumpThreshold = 0.7f;
    [SerializeField] private float jumpSpeed = 5;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        navMeshTargetAgent = navMeshTarget.GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(float damageAmount){
        health -= damageAmount;
        animator.SetTrigger("damage");

        if(health <= 0){
            Die();
        }

    }

    public void Die(){
        Destroy(this.transform.parent.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToTarget();
        RotateToTarget();
    }

    void Update(){
        if(isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinnishTime){
            isAttacking = false;
        }

        if(!isAttacking){
            float distance = Vector3.Distance(transform.position, playerTarget.position);
            if(distance < attackRange){
                Attack();
            }
            
        }
        else{   
            Debug.Log("Can't attack");
        }
    }

    private void Attack()
    {
        animator.SetTrigger("attack");
        StopCoroutine(ResetAttackWaiting());
        StartCoroutine(ResetAttackWaiting());
    }

    private void RotateToTarget(){

        if(playerTarget.rotation != null){
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (playerTarget.position - transform.position), rotationSpeed * Time.fixedDeltaTime);
            
            if(!narutoRun){
                targetRotation.x = 0;
                targetRotation.z = 0;
            }
            
            targetRotation.Normalize();
            rb.MoveRotation(targetRotation);
        }
        else{
            rb.MoveRotation(navMeshTarget.rotation);
        }
    }

    private void MoveToTarget(){
        
        Vector3 targetDirection = navMeshTarget.position - transform.position;

        float distance = Vector3.Distance(transform.position, navMeshTarget.position);

        if(distance < stoppingDistance){
            return;
        }

        targetDirection.Normalize();

        Vector3 movementSpeed = this.movementSpeed;
        if(navMeshTarget.position.y - transform.position.y > jumpThreshold){
            movementSpeed.y = jumpSpeed;
        }
        
        Debug.Log(movementSpeed);
        rb.MovePosition(transform.position + 
            (Vector3.Scale(targetDirection, movementSpeed) * Time.fixedDeltaTime));

        animator.SetFloat("speed", navMeshTargetAgent.velocity.magnitude / navMeshTargetAgent.speed);

    }

    IEnumerator ResetAttackWaiting(){

        yield return new WaitForSeconds(0.1f);
        isAttacking = true;

    }
}
