using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class HealthSystem : MonoBehaviour
{
    protected Animator animator;
    public float health = 3f;

    // Start is called before the first frame update
    void Awake()
    {
        animator = transform.GetComponentInChildren<Animator>();

        
    }
    public abstract void TakeDamage(float damageAmount);

    
    public abstract void Die();


    public void Setup(Animator animator){
        this.animator = animator;
    }

    
}
