using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageDealer : MonoBehaviour
{

    bool canDealDamage;
    bool hasDealtDamage;
    [SerializeField] float weaponDamage;
    // Start is called before the first frame update
    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = false;
    }

    public void StartDealDamage(){
        canDealDamage = true;
        hasDealtDamage = false;
    }

    public void EndDealDamage(){
        canDealDamage = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if(!canDealDamage || hasDealtDamage){
            return;
        }

        HealthSystem healthSystem;

        if(!other.transform.TryGetComponent<HealthSystem>(out healthSystem)){
            return;
        }

        hasDealtDamage = true;
        healthSystem.TakeDamage(weaponDamage);
        
    }
}
