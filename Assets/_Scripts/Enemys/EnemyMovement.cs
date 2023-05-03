using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform target;
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

        if(enabled){
            if(Vector3.Distance(target.position, enemy.position) <= targetMaxDistance){
                agent.SetDestination(target.position);
            }
        }
        
        followCoroutine = StartCoroutine(FollowTarget());
        
        
    }
}
