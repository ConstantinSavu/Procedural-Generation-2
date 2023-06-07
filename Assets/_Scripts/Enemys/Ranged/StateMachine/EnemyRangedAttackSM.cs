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
    [SerializeField] public NavMeshEnemyMovement navMeshEnemyMovement;

    private void Awake() {
        
        idleRangedAttack = new IdleRangedAttack(this);
        drawRangedAttack = new DrawRangedAttack(this);
        holdRangedAttack = new HoldRangedAttack(this);
        fireRangedAttack = new FireRangedAttack(this);

        crossBow = GetComponentInChildren<Crossbow>();
        navMeshEnemyMovement = GetComponentInChildren<NavMeshEnemyMovement>();
    }

    protected override BaseState GetInitialState(){
        return idleRangedAttack;
    }

    private void OnGUI(){
        string content = CurrentState != null ? CurrentState.ToString() : "(No current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }

    
}
