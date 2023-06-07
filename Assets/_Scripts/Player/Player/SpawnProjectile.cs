using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;    
    [SerializeField] private GameObject spawnProjectile;

    [SerializeField] private Vector3 speed = new Vector3(20f, 20f, 20f);

    void Start()
    {
        playerInput.OnMouseRightClick += HandleRightMouseClick;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void HandleRightMouseClick(){

        Projectile projectile;

        GameObject instantiatedProjectile = Instantiate(spawnProjectile, transform.position, transform.rotation);

        if(instantiatedProjectile.transform.TryGetComponent(out projectile)){

        }

    }

}
