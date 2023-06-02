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

    public Material skinMaterial;

    public Color damageColor;
    public float changeColorTime = 0.5f;

    public float damping = 1f;
    public float health = 3;
    public float attackRange = 1.5f;
    private NavMeshAgent agent;
    Animator animator;
    EnemyDamageDealer enemyDamageDealer = null;
    public float animationFinnishTime = 0.9f;
    private Coroutine followCoroutine;
    private bool isAttacking = false;
    private bool dealtDamage = false;

    class ColorChangeRenderer{
        public Renderer renderer;
        public Color originalColor;

        public ColorChangeRenderer(Renderer renderer, Color originalColor){

            this.renderer = renderer;
            this.originalColor = originalColor;
            
        }

    }
    List<ColorChangeRenderer> colorChangeRenderers = new List<ColorChangeRenderer>();
    
    

    private void Awake(){
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in renderers)
        {
            try{
                Color originalColor = renderer.material.GetColor("_OTHERCOLOR");

                ColorChangeRenderer colorChangeRenderer = new ColorChangeRenderer(renderer, originalColor);
                colorChangeRenderers.Add(colorChangeRenderer);
            }
            catch(Exception e){
                Debug.Log(e.Message);
            }
        }
 
    }

    public void Start(){
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
        StartCoroutine(ChangeColor(damageColor));
        if(health <= 0){
            Die();
        }

    }

    public void Die(){
        Destroy(this.transform.parent.gameObject);
    }
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

    IEnumerator ChangeColor(Color color){

        foreach(ColorChangeRenderer colorChangeRenderer in colorChangeRenderers)
        {
            colorChangeRenderer.renderer.material.SetColor("_OTHERCOLOR", color);
        }

        yield return new WaitForSeconds(changeColorTime);

        foreach(ColorChangeRenderer colorChangeRenderer in colorChangeRenderers)
        {
            colorChangeRenderer.renderer.material.SetColor("_OTHERCOLOR", colorChangeRenderer.originalColor);
        }

    }

    private IEnumerator FollowTarget()
    {
        yield return new WaitForSeconds(updateTargetSpeed);
        
        bool setDestination = true;

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
        
        if(setDestination){
            agent.SetDestination(target.position);

        }

        followCoroutine = StartCoroutine(FollowTarget());
        
        
    }
}
