using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshEnemyMovement : MonoBehaviour
{
    public Transform target;
    public Character character = null;
    public float targetMaxDistance = 40f;
    public float updateTargetSpeed = 0.1f;
    public NavMeshAgent agent;
    Animator animator;

    EnemyDamageDealer enemyDamageDealer = null;

    public float damping = 1f;
    public float health = 3;
    public float attackRange = 1.5f;
    
    public float animationFinnishTime = 0.9f;

    private Coroutine followCoroutine;
    private bool isAttacking = false;
    private bool dealtDamage = false;

    private void Awake(){
        animator = GetComponent<Animator>();
    }

    public void Start(){
        if(character == null && target != null){
            target.gameObject.TryGetComponent<Character>(out character);
        }
        StartFollowing();
    }

    public void Update(){
        
        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);
        

        if(isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinnishTime){
            isAttacking = false;
            dealtDamage = false;
        }

        if(!isAttacking && !dealtDamage && target != null){
            float distance = Vector3.Distance(transform.position, target.position);
            if(distance < attackRange){
                Attack();
            }
            
        }

        if(target != null && Vector3.Distance(target.position, transform.position) <= targetMaxDistance){
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
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

    int counter = 0;
    private void Attack()
    {
        dealtDamage = true;
        animator.SetTrigger("isAttacking");
        StopCoroutine(ResetAttackWaiting());
        StartCoroutine(ResetAttackWaiting());
    }

    public void StartDealDamage(){
        if(enemyDamageDealer == null){
            enemyDamageDealer = GetComponentInChildren<EnemyDamageDealer>();
        }

        enemyDamageDealer.StartDealDamage();
    }

    public void EndDealDamage(){
        if(enemyDamageDealer == null){
            enemyDamageDealer = GetComponentInChildren<EnemyDamageDealer>();
        }
        enemyDamageDealer.EndDealDamage();
    }


    public void StartFollowing(){
        if(followCoroutine == null){
            followCoroutine = StartCoroutine(FollowTarget());
            return;
        }

        Debug.Log("Enemy already following");
        
    }

    IEnumerator ResetAttackWaiting(){

        yield return new WaitForSeconds(0.1f);
        isAttacking = true;

    }

    private IEnumerator FollowTarget()
    {
        yield return new WaitForSeconds(updateTargetSpeed);
        
        if(enabled && target != null){
        if(Vector3.Distance(target.position, transform.position) <= targetMaxDistance){
        if(character != null && !character.inWater){
            
            agent.SetDestination(target.position);
            
        }
        }
        }
        
        followCoroutine = StartCoroutine(FollowTarget());
    }
}
