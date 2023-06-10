using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshEnemyMovement))]

[RequireComponent(typeof(EnemyHealthSystem))]


public class Enemy : PoolableObject
{
    
    
    public Transform target;
    [SerializeField] EnemyType enemyType;

    NavMeshEnemyMovement navMeshEnemyMovement;
    EnemyAttack enemyAttack;

    EnemyHealthSystem enemyTakeDamage;

    Collider Collider;

    Animator animator;
    public UnityEvent<EnemyType> onDie;


    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshEnemyMovement = GetComponent<NavMeshEnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyTakeDamage = GetComponent<EnemyHealthSystem>();
        Collider = GetComponent<Collider>();

        if(target != null){
            StartupEnemy(target);
        }
    }

    public void StartupEnemy(Transform target){
        this.target = target;
        
        navMeshEnemyMovement.Setup(target, animator);
        enemyAttack.Setup(target, animator);

        enemyTakeDamage.Setup(animator);
    }

    public void StartupEnemy(Transform target, NavMeshHit hit){
        
        this.target = target;

        navMeshEnemyMovement.Setup(target, animator, hit);
        enemyAttack.Setup(target, animator);

        enemyTakeDamage.Setup(animator);
    }

    public void EnemyDeath(){
        animator.SetLayerWeight(1, 0f);
        animator.SetTrigger("death");

        navMeshEnemyMovement.DiableAgent();
        navMeshEnemyMovement.enabled = false;
        enemyAttack.enabled = false;
        enemyTakeDamage.enabled = false;
        Collider.enabled = false;
        onDie?.Invoke(enemyType);
        
        StartCoroutine(DeathTimer(5f));
    }

    public override void OnDisable()
    {   
        
        navMeshEnemyMovement.enabled = false;
        enemyAttack.enabled = false;
        enemyTakeDamage.enabled = false;
        Collider.enabled = false;
        base.OnDisable();
        
    }

    private void OnEnable() {
        
        navMeshEnemyMovement.enabled = true;
        enemyAttack.enabled = true;
        enemyTakeDamage.enabled = true;
        Collider.enabled = true;
    }

    protected IEnumerator DeathTimer(float seconds){
        yield return new WaitForSeconds(seconds);
        transform.gameObject.SetActive(false);
        
    }


}
