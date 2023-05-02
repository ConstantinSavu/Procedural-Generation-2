using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform target;

    public float updateTargetSpeed = 0.1f;
    public NavMeshAgent agent;

    public Coroutine followCoroutine;

    private void Awake(){
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

        if(enabled){
            Debug.Log("Enemy set destination after " + updateTargetSpeed);
            agent.SetDestination(target.position);
        }
        
        followCoroutine = StartCoroutine(FollowTarget());
        
        
    }
}
