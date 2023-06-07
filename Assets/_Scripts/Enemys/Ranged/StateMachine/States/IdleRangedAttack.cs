using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRangedAttack : BaseState
{
    EnemyRangedAttackSM _sm;
    private float animationFinnishTime = 0.9f;
    private float transitionFinnishTime = 0.9f;

    public IdleRangedAttack(EnemyRangedAttackSM stateMachine) : base(stateMachine){
        _sm = (EnemyRangedAttackSM)stateMachine;
    }
    public override void Enter()
    {
        base.Enter();
        _sm.animator.ResetTrigger("draw");
        _sm.animator.ResetTrigger("fire");

        _sm.crossBow.ReadyToFire = false;
        _sm.crossBow.ShowArrow = true;
        _sm.crossBow.UpdateRotationToTarget = false;
        _sm.crossBow.CheckCollisionToTarget = false;

    }

    public override void Update()
    {
        base.Update();
        
        float distance = Vector3.Distance(_sm.transform.position, _sm.target.position);
        if(distance < _sm.attackRange){
            stateMachine.ChangeState(_sm.drawRangedAttack);    
        }

    }
}
