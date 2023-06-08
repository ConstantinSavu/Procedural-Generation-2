using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    [SerializeField] bool canDealDamage;
    [SerializeField] bool hasDealtDamage;

    [SerializeField] float weaponLength;
    [SerializeField] float weaponDamage;
    [SerializeField] LayerMask damageLayers;
    // Start is called before the first frame update
    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(canDealDamage && !hasDealtDamage){
            RaycastHit hit;
            
            if(Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, damageLayers.value)){
                
                hasDealtDamage = true;
                HealthSystem healthSystem;
                if(hit.transform.TryGetComponent<HealthSystem>(out healthSystem)){
                    healthSystem.TakeDamage(weaponDamage);
                }
                
            }
        }
    }

    public void StartDealDamage(){
        canDealDamage = true;
        hasDealtDamage = false;
    }

    public void EndDealDamage(){
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}
