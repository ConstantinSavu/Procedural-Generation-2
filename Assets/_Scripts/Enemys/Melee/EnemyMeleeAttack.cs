using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : EnemyAttack
{
    [SerializeField] private float animationFinnishTime = 1f;
    private bool isAttacking = false;
    EnemyDamageDealer enemyDamageDealer = null;

    public void Update(){
        if(isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinnishTime){
            isAttacking = false;

        }

        if(!isAttacking && target != null){
            float distance = Vector3.Distance(transform.position, target.position);
            if(distance < attackRange){
                Attack();
            }
            
        }
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

    protected virtual void Attack()
    {
        animator.SetTrigger("isAttacking");

        StopCoroutine(ResetAttackWaiting());
        StartCoroutine(ResetAttackWaiting());
    }

    protected IEnumerator ResetAttackWaiting(){

        yield return new WaitForSeconds(0.1f);
        isAttacking = true;

    }
}
