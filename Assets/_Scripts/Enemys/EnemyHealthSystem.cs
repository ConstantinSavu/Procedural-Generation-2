using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyType{
        Melee,
        Ranged,
        Boss

}
public class EnemyHealthSystem : HealthSystem
{
    List<ColorChangeRenderer> colorChangeRenderers = new List<ColorChangeRenderer>();
    [SerializeField] String originalColorName = "_BaseColor";
    
    public Color damageColor = Color.red;
    public float changeColorTime = 0.5f;

    
    Enemy enemy;
    

    void Awake()
    {

        enemy = GetComponent<Enemy>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        animator = transform.GetComponentInChildren<Animator>();

        foreach(Renderer renderer in renderers)
        {
            try{
                Color originalColor = renderer.material.GetColor(originalColorName);

                ColorChangeRenderer colorChangeRenderer = new ColorChangeRenderer(renderer, originalColor);
                colorChangeRenderers.Add(colorChangeRenderer);
            }
            catch(Exception e){
                Debug.Log(e.Message);
            }
        }
    }

    public override void TakeDamage(int damageAmount){
        health -= damageAmount;
        
        
        
        StartCoroutine(ChangeColor(damageColor));
        
        if(health <= 0){
            Die();
            return;
        }

        animator.SetTrigger("damage");

    }

    public override void Die(){
        
        if(enemy == null){
            enemy = GetComponent<Enemy>();
        }

        
        enemy.EnemyDeath();
    }

    IEnumerator ChangeColor(Color color){

        foreach(ColorChangeRenderer colorChangeRenderer in colorChangeRenderers)
        {
            colorChangeRenderer.renderer.material.SetColor(originalColorName, color);
        }

        yield return new WaitForSeconds(changeColorTime);

        foreach(ColorChangeRenderer colorChangeRenderer in colorChangeRenderers)
        {
            colorChangeRenderer.renderer.material.SetColor(originalColorName, colorChangeRenderer.originalColor);
        }

    }

    private void OnValidate() {
        
    }
}

class ColorChangeRenderer{
    public Renderer renderer;
    public Color originalColor;
    public ColorChangeRenderer(Renderer renderer, Color originalColor){

        this.renderer = renderer;
        this.originalColor = originalColor;
        
    }

}
