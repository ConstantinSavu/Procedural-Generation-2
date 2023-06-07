using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    protected Animator animator;
    public float health = 3f;
    // Start is called before the first frame update
    void Awake()
    {
        animator = transform.GetComponentInChildren<Animator>();
    }
    public virtual void TakeDamage(float damageAmount){
        health -= damageAmount;
        animator.SetTrigger("damage");

        if(health <= 0){
            Die();
        }

    }

    public void Setup(Animator animator){
        this.animator = animator;
    }

    public void Die(){
        Debug.Log("You have died");
        health = 3;
    }
}
