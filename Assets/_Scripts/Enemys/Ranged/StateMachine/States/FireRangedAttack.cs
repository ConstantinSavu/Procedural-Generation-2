using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRangedAttack : BaseState
{
    EnemyRangedAttackSM _sm;
    private float animationFinnishTime = 0.9f;

    private float shootArrowTime = 0.1f;
    public FireRangedAttack(EnemyRangedAttackSM stateMachine) : base(stateMachine){
        _sm = (EnemyRangedAttackSM)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _sm.crossBow.ReadyToFire = true;
        _sm.crossBow.ShowArrow = true;
        _sm.crossBow.UpdateRotationToTarget = true;
        _sm.crossBow.ShootArrow();
        _sm.animator.SetTrigger("fire");
        
        
        
    }

    public override void Exit()
    {
        base.Exit();
        _sm.animator.ResetTrigger("fire");
        _sm.crossBow.UpdateRotationToTarget = false;
        
    }

    public override void Update()
    {
        base.Update();

        if(_sm.animator.GetCurrentAnimatorStateInfo(1).IsName("Crosbow_Fire") &&
            _sm.animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= shootArrowTime){
            if(_sm.crossBow.ReadyToFire){
                
            }
        }

        if(_sm.animator.GetCurrentAnimatorStateInfo(1).IsName("Crosbow_Fire") &&
            _sm.animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinnishTime){
            stateMachine.ChangeState(_sm.idleRangedAttack);
        }
    }
}
