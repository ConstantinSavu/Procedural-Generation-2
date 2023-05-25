using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshEnemyMovement : MonoBehaviour
{
    public GameObject target;
    public Character character = null;
    public Transform navMeshEnemy;
    public Transform rigidBodyEnemy;
    public float targetMaxDistance = 40f;
    public float maxDistanceBtwnAgentAndBody = 1f;

    public float updateTargetSpeed = 0.1f;
    public NavMeshAgent agent;
    [SerializeField]
    public NavMeshPath path;

    public Coroutine followCoroutine;

    private void Awake(){
        navMeshEnemy = this.GetComponent<Transform>();
        path = new NavMeshPath();
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
        
        if(Vector3.Distance(rigidBodyEnemy.position, navMeshEnemy.position) > maxDistanceBtwnAgentAndBody){
            agent.Warp(rigidBodyEnemy.position);
        }

        if(Vector3.Distance(target.transform.position, navMeshEnemy.position) <= targetMaxDistance){
            
            agent.CalculatePath(target.transform.position, path);
            agent.SetPath(path);
            foreach(var corner in path.corners){
                Debug.Log(corner);
            }
            
        }
        }
        
        followCoroutine = StartCoroutine(FollowTarget());
        
        
    }
}
