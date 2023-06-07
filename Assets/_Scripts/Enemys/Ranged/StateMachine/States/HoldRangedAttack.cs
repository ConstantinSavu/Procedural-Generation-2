
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldRangedAttack : BaseState
{
    EnemyRangedAttackSM _sm;

    
    public HoldRangedAttack(EnemyRangedAttackSM stateMachine) : base(stateMachine){
        _sm = (EnemyRangedAttackSM)stateMachine;
    }

     public override void Enter()
    {
        base.Enter();
        _sm.crossBow.ReadyToFire = false;
        _sm.crossBow.ShowArrow = true;
        _sm.crossBow.UpdateRotationToTarget = true;
        _sm.crossBow.CheckCollisionToTarget = false;
        _sm.crossBow.StartHoldCountDown(1f);
        
    }

    public override void Exit()
    {
        base.Exit();
        
    }

    public override void Update()
    {
        base.Update();

        if(_sm.crossBow.ReadyToFire == true){
            
            stateMachine.ChangeState(_sm.fireRangedAttack);
        }
    }
}
