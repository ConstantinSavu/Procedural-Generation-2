using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossAttack : EnemyAttack
{
    [SerializeField] private float animationFinnishTime = 1f;
    [SerializeField] private float rangedAttackRange = 10f;
    [SerializeField] private float rangedAttackCooldown = 10f;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isRangeAttacking = false;
    [SerializeField] private bool canRangeAttack = true;

    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawnPoint;

    [SerializeField] Vector3 minRotationRelativeToSpawnPoint = new Vector3(30f, 30f, 30f);
    [SerializeField] Vector3 maxRotationRelativeToSpawnPoint = new Vector3(30f, 30f, 30f);
    EnemyDamageDealer enemyDamageDealer = null;

    NavMeshEnemyMovement navMeshEnemyMovement;

    private void Start() {
        navMeshEnemyMovement = transform.GetComponent<NavMeshEnemyMovement>();
    }

    public void Update(){
        if(isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinnishTime){
            isAttacking = false;

        }

        if(isRangeAttacking && animator.GetCurrentAnimatorStateInfo(2).normalizedTime >= animationFinnishTime){
            isRangeAttacking = false;
            

        }

        if(!isRangeAttacking && !isAttacking && target != null){
            float distance = Vector3.Distance(transform.position, target.position);
            if(distance <= attackRange){
                Attack();
            }
            
        }

        if(canRangeAttack && !isRangeAttacking && !isAttacking && target != null){
            float distance = Vector3.Distance(transform.position, target.position);
            if(attackRange < distance && distance < rangedAttackRange){
                RangedAttack();
                
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

    public void Throw(){
        Vector3 lookPos = target.position - projectileSpawnPoint.position;
        
        lookPos.Normalize();

        if(lookPos != Vector3.zero){
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            
            Vector3 rotationBeforeClamp = rotation.eulerAngles;
            Vector3 holderRotation = transform.eulerAngles;

            Vector3 minBounds = (holderRotation - minRotationRelativeToSpawnPoint);

            Vector3 maxBounds = (holderRotation + maxRotationRelativeToSpawnPoint);

            Vector3 rotationAfterClamp = ClampRotation(rotationBeforeClamp, minBounds, maxBounds);

            rotation = Quaternion.Euler(rotationAfterClamp);

            PhysicalFireBall physicalFireBall;
            GameObject spawnedArrow = Instantiate(projectile, projectileSpawnPoint.position, rotation);
            physicalFireBall = spawnedArrow.transform.GetComponent<PhysicalFireBall>();
            physicalFireBall.target = target;
            
        }
    }

    private Vector3 ClampRotation(Vector3 rotationEulerAngles, Vector3 minBounds, Vector3 maxBounds){

        rotationEulerAngles.x = ClampAngle(rotationEulerAngles.x, minBounds.x, maxBounds.x);
        rotationEulerAngles.y = ClampAngle(rotationEulerAngles.y, minBounds.y, maxBounds.y);
        rotationEulerAngles.z = ClampAngle(rotationEulerAngles.z, minBounds.z, maxBounds.z);
        
        return rotationEulerAngles;
    }

    private float ClampAngle(float angle, float minAngle, float maxAngle){
        
        float minAngle_InCircleRange = Mathf.Repeat(minAngle, 360f);
        float maxAngle_InCircleRange = Mathf.Repeat(maxAngle, 360f);
        float angle_InCircleRange = Mathf.Repeat(angle, 360f);

        float offset = 360f - minAngle_InCircleRange;

        //float offsetMinAngle = 0f; //is implicit
        float offsetMaxAngle = Mathf.Repeat(maxAngle_InCircleRange + offset, 360f);
        float offsetAngle = Mathf.Repeat(angle_InCircleRange + offset, 360f);

        if(offsetAngle <= offsetMaxAngle){
            return angle_InCircleRange;
        }

        float minDistance = 360f - offsetAngle;
        float maxDistance = offsetAngle - offsetMaxAngle;

        if(minDistance < maxDistance){
            return minAngle_InCircleRange;
        }

        return maxAngle_InCircleRange;
    }

    protected virtual void Attack()
    {
        animator.SetTrigger("isAttacking");

        StopCoroutine(ResetAttackWaiting());
        StartCoroutine(ResetAttackWaiting());
    }

    private void RangedAttack()
    {
        animator.SetTrigger("rangedAttack");
        canRangeAttack = false;

        StopCoroutine(ResetRangedAttackWaiting());
        StartCoroutine(ResetRangedAttackWaiting());

        StopCoroutine(RangedAttackCooldown());
        StartCoroutine(RangedAttackCooldown());
    }

    private IEnumerator RangedAttackCooldown()
    {
        yield return new WaitForSeconds(rangedAttackCooldown);
        canRangeAttack = true;
    }

    protected IEnumerator ResetAttackWaiting(){

        yield return new WaitForSeconds(0.1f);
        isAttacking = true;

    }

    protected IEnumerator ResetRangedAttackWaiting(){

        yield return new WaitForSeconds(0.1f);
        isAttacking = true;

    }
}
