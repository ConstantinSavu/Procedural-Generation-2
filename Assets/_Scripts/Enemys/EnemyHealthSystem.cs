using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealthSystem : HealthSystem
{
    List<ColorChangeRenderer> colorChangeRenderers = new List<ColorChangeRenderer>();
    [SerializeField] String originalColorName = "_BaseColor";
    public UnityEvent onDie; 
    public Color damageColor = Color.red;
    public float changeColorTime = 0.5f;

    void Awake()
    {
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

    public new void TakeDamage(float damageAmount){
        health -= damageAmount;
        animator.SetTrigger("damage");
        StartCoroutine(ChangeColor(damageColor));
        if(health <= 0){
            Die();
        }

    }

    public new void Die(){
        onDie?.Invoke();
        transform.gameObject.SetActive(false);
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
