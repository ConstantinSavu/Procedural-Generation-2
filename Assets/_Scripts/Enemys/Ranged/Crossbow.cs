using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crossbow : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] Transform arrow;
    [SerializeField] GameObject physicalProjectile;
    [SerializeField] Transform holder;
    [SerializeField] public float damping = 1f;

    [SerializeField] Vector3 minRotationRelativeToHolder = new Vector3(30f, 30f, 30f);
    [SerializeField] Vector3 maxRotationRelativeToHolder = new Vector3(30f, 30f, 30f);
    [SerializeField] public bool UpdateRotationToTarget;
    [SerializeField] public bool ReadyToFire;
    [SerializeField] public bool ShowArrow;
    [SerializeField] public bool CheckCollisionToTarget = false;
    

    // Start is called before the first frame update

    // Update is called once per frame

    void Awake()
    {

    }

    void Update()
    {
        if(target == null){
            target = holder.GetComponent<EnemyAttack>().target;
        }

        if(UpdateRotationToTarget && target != null){
            AimCrossBowAtTarget();
        }

        if(CheckCollisionToTarget && target != null){
            CheckCollision();
        }
        
        arrow.gameObject.SetActive(ShowArrow);
        arrow.rotation = transform.rotation;
    }

    private void CheckCollision()
    {
        if(Physics.Raycast(arrow.position, arrow.forward, 100f, 1 << target.gameObject.layer)){
            ReadyToFire = true;
            ShootArrow();
        }
    }

    void AimCrossBowAtTarget(){
        
        Vector3 lookPos = target.position - arrow.position;
        
        lookPos.Normalize();

        
        if(lookPos != Vector3.zero){
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            
            Vector3 rotationBeforeClamp = rotation.eulerAngles;
            Vector3 holderRotation = holder.eulerAngles;

            Vector3 minBounds = (holderRotation - minRotationRelativeToHolder);

            Vector3 maxBounds = (holderRotation + maxRotationRelativeToHolder);

            Vector3 rotationAfterClamp = ClampRotation(rotationBeforeClamp, minBounds, maxBounds);

            rotation = Quaternion.Euler(rotationAfterClamp);
            
            arrow.rotation = Quaternion.Slerp(arrow.rotation, rotation, Time.deltaTime * damping);
            transform.rotation = arrow.rotation;
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


    

    public void ShootArrow(){

        Projectile projectile;
        GameObject spawnedArrow = Instantiate(physicalProjectile, arrow.position, arrow.rotation);
        projectile = spawnedArrow.transform.GetComponent<Projectile>();
        projectile.Shoot();

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(arrow.position, arrow.position + arrow.forward * 100);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 100);
    }

    public void StartHoldCountDown(float delay){
        StopCoroutine(HoldCountDown(delay));
        StartCoroutine(HoldCountDown(delay));
    }

    private IEnumerator HoldCountDown(float delay){
        
        yield return new WaitForSeconds(delay);
        CheckCollisionToTarget = true;
    }
    
}
