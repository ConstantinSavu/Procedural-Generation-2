using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshEnemyMovement : MonoBehaviour
{
    public Transform target;
    public float targetMaxDistance = 40f;
    public float updateTargetSpeed = 0.1f;
    public float damping = 1f;    
    private NavMeshAgent agent;
    private Animator animator;
    private Coroutine followCoroutine;

    public float TimeToLive = 100;

    [SerializeField] bool setDestination;

    private void Awake(){
        
        agent = GetComponent<NavMeshAgent>();
        
    }

    public void Setup(Transform target, Animator animator){

        this.target = target;
        this.animator = animator;
        agent.enabled = true;
        this.StartFollowing();

    }

    public void Setup(Transform target, Animator animator, NavMeshHit hit){

        this.target = target;
        this.animator = animator;
        agent.Warp(hit.position);
        agent.enabled = true;
        this.StartFollowing();

    }

    public void Update(){
        if(animator != null){
            animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);
        }

        if(target != null && Vector3.Distance(target.position, transform.position) <= targetMaxDistance){
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0;
            if(lookPos != Vector3.zero){
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            }
            
        }
    }

    public void StartFollowing(){
        if(followCoroutine == null){
            followCoroutine = StartCoroutine(FollowTarget());
            return;
        }

        Debug.Log("Enemy already following");
        
    }

    public void StopMovement(){
        agent.isStopped = true;
    }

    public void StartMovement(){
        agent.isStopped = false;
    }

    private IEnumerator FollowTarget()
    {
        yield return new WaitForSeconds(updateTargetSpeed);
        
        setDestination = true;

        if(!enabled){
            setDestination = false;
        }

        if(target == null){
            Debug.Log("Awaiting target");
            setDestination = false;
        }

        if(Vector3.Distance(target.position, transform.position) > targetMaxDistance){
            setDestination = false;
        }

        if(agent.isStopped){
            setDestination = false;
        }
        
        if(setDestination){
            try{
                agent.SetDestination(target.position);

                if(!agent.hasPath){
                
                }
                
            }
            catch(Exception e){
                Debug.Log(e.Message);
                Debug.Log(transform.name);
            }

        }

        followCoroutine = StartCoroutine(FollowTarget());
        
        
    }

    IEnumerator dieTimer(float time){
        yield return new WaitForSeconds(time);
        transform.gameObject.SetActive(false);
    }

    internal void DiableAgent()
    {
        agent.enabled = false;
    }

    internal void EnableAgent()
    {
        agent.enabled = true;
    }
}
