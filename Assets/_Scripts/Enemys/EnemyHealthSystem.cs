using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    class ColorChangeRenderer{
        public Renderer renderer;
        public Color originalColor;
        public ColorChangeRenderer(Renderer renderer, Color originalColor){

            this.renderer = renderer;
            this.originalColor = originalColor;
            
        }

    }
    List<ColorChangeRenderer> colorChangeRenderers = new List<ColorChangeRenderer>();
    
    public float health = 3;
    [SerializeField] String originalColorName = "_OTHERCOLOR";

    public Color damageColor;
    public float changeColorTime = 0.5f;

    public Animator animator;

    void Awake()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

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

    public void TakeDamage(float damageAmount){
        health -= damageAmount;
        animator.SetTrigger("damage");
        StartCoroutine(ChangeColor(damageColor));
        if(health <= 0){
            Die();
        }

    }

    public void Die(){
        if(transform.parent != null){
            Destroy(transform.parent.gameObject);
            return;
        }
        Destroy(transform.gameObject);
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
}
