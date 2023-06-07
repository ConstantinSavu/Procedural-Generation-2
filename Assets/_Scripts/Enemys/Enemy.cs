using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavMeshEnemyMovement))]

[RequireComponent(typeof(EnemyHealthSystem))]

public class Enemy : MonoBehaviour
{
    public Transform target;

    NavMeshEnemyMovement navMeshEnemyMovement;
    EnemyAttack enemyAttack;

    EnemyHealthSystem enemyTakeDamage;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshEnemyMovement = GetComponent<NavMeshEnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyTakeDamage = GetComponent<EnemyHealthSystem>();

        if(target != null){
            InstantiateEnemy(target);
        }
    }

    public void InstantiateEnemy(Transform target){
        this.target = target;
        navMeshEnemyMovement.target = target;
        navMeshEnemyMovement.animator = animator;
        navMeshEnemyMovement.StartFollowing();

        enemyAttack.target = target;
        enemyAttack.animator = animator;

        enemyTakeDamage.animator = animator;


    }
}
