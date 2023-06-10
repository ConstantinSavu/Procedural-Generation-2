using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyRangedAttackSM : EnemyAttack
{
    public IdleRangedAttack idleRangedAttack;
    public DrawRangedAttack drawRangedAttack;
    public HoldRangedAttack holdRangedAttack;
    public FireRangedAttack fireRangedAttack;

    [SerializeField] public Crossbow crossBow;
    

    private void Awake() {
        
        idleRangedAttack = new IdleRangedAttack(this);
        drawRangedAttack = new DrawRangedAttack(this);
        holdRangedAttack = new HoldRangedAttack(this);
        fireRangedAttack = new FireRangedAttack(this);

        crossBow = GetComponentInChildren<Crossbow>();
        
    }

    private void OnDisable() {
        crossBow.enabled = false;
    }

    private void OnEnable() {
        crossBow.enabled = true;
    }

    protected override BaseState GetInitialState(){
        return idleRangedAttack;
    }
    
}
