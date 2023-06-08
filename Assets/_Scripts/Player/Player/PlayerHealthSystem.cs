using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthSystem : HealthSystem
{
    public UnityEvent onPlayerDie;

    public void Setup(Animator animator, UnityAction callback){
        this.animator = animator;
        onPlayerDie.AddListener(callback);
    }

    public void Setup(UnityAction callback){
        
        Debug.Log("Added Lisener " + callback);
        onPlayerDie.AddListener(callback);

    }

    public override void TakeDamage(float damageAmount){

        Debug.Log("Player hit");
        health -= damageAmount;
        animator.SetTrigger("damage");
        
        if(health <= 0){
            Die();
        }

    }

    public override void Die(){
        Debug.Log("PlauerDied");
        onPlayerDie?.Invoke();
    }
}
