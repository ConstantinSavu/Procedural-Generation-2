using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : StateMachine
{
    public Animator animator;
    public Transform target;
    public float attackRange = 1.5f;

    public void Setup(Transform target, Animator animator){
        this.target = target;
        this.animator = animator;
    }
}
