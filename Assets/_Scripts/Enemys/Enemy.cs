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

    Animator animator;
    public UnityEvent<EnemyType> onDie;


    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshEnemyMovement = GetComponent<NavMeshEnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyTakeDamage = GetComponent<EnemyHealthSystem>();

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

    public override void OnDisable()
    {
        base.OnDisable();
        onDie?.Invoke(enemyType);
        navMeshEnemyMovement.DiableAgent();

    }


}
