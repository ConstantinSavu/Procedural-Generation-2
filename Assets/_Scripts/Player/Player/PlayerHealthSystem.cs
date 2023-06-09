using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthSystem : HealthSystem
{

    public int maxHealth = 10;
    public float regenerateTime = 5f;
    public UnityEvent onPlayerDie;

    [SerializeField] HeartManager fullHeartsManger;
    [SerializeField] HeartManager emptyHeartsManager;

    Coroutine regenerateHealthCoroutine;
    
    void Awake() {
        
        fullHeartsManger.InstantiateHearts(health);
        emptyHeartsManager.InstantiateHearts(maxHealth);
        

    }
    public void Setup(Animator animator, UnityAction callback){
        this.animator = animator;
        onPlayerDie.AddListener(callback);
    }

    public void Setup(UnityAction callback){
        
        Debug.Log("Added Lisener " + callback);
        onPlayerDie.AddListener(callback);

    }

    void Update() {
        if(regenerateHealthCoroutine == null && health < maxHealth){
            regenerateHealthCoroutine = StartCoroutine(RegenerateHealth(regenerateTime));
        }
    }

    

    public override void TakeDamage(int damageAmount){

        Debug.Log("Player hit");
        health -= damageAmount;

        if(animator == null){
            animator = GetComponentInChildren<Animator>();
        }

        for(int i = 0; i < damageAmount; i++){
            fullHeartsManger.RemoveHeart();
        }

        if(regenerateHealthCoroutine != null){
            StopCoroutine(regenerateHealthCoroutine);
            regenerateHealthCoroutine = null;
        }
        

        animator.SetTrigger("damage");

        

        if(health <= 0){
            Die();
        }

    }

    public override void Die(){
        Debug.Log("PlauerDied");
        onPlayerDie?.Invoke();
    }

    IEnumerator RegenerateHealth(float regenerateTime){
        
        WaitForSeconds wait = new WaitForSeconds(regenerateTime);
        yield return wait;
        health++;
        fullHeartsManger.AddHeart();
        regenerateHealthCoroutine = null;
        
    }
}
