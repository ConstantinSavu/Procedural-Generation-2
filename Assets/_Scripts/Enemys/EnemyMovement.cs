using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public GameObject target;
    public Character character = null;
    public Transform enemy;
    public float targetMaxDistance = 40f;

    public float updateTargetSpeed = 0.1f;
    public NavMeshAgent agent;

    public Coroutine followCoroutine;

    private void Awake(){
        enemy = this.GetComponent<Transform>();
    }

    public void StartFollowing(){
        if(followCoroutine == null){
            followCoroutine = StartCoroutine(FollowTarget());
            return;
        }

        Debug.Log("Enemy already following");
        
    }

    private IEnumerator FollowTarget()
    {
        yield return new WaitForSeconds(updateTargetSpeed);

        if(character == null){
            character = target.transform.GetComponent<Character>();
        }

        if(enabled){
        if(Vector3.Distance(target.transform.position, enemy.position) <= targetMaxDistance){
        if(!character.inWater)
            agent.SetDestination(target.transform.position);
        }
        }
        
        followCoroutine = StartCoroutine(FollowTarget());
        
        
    }
}
