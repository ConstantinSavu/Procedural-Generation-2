using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;    
    [SerializeField] private GameObject spawnProjectile;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Vector3 baseSpeedModifier = new Vector3(1f, 1f, 1f);

    [SerializeField] private Vector3 maxBaseSpeed = new Vector3(20f, 20f, 20f);

    void Start()
    {
        playerInput.OnMouseRightClick += HandleRightMouseClick;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int counter = 0;
    private void HandleRightMouseClick(){
        
        if(PauseManager.gameIsPaused){
            return;
        }

        Projectile projectile;

        GameObject instantiatedProjectile = Instantiate(spawnProjectile, transform.position, transform.rotation);
        instantiatedProjectile.name = "Sticky Light " + counter++;
        if(instantiatedProjectile.transform.TryGetComponent(out projectile)){
            Vector3 velocity;
            velocity = controller.velocity;

            Vector3 scaledVelocity = Vector3.Scale(velocity, baseSpeedModifier);
            scaledVelocity = Vector3Clamp(scaledVelocity, Vector3.zero, maxBaseSpeed);

            projectile.Shoot(scaledVelocity);
        }

    }

    private Vector3 Vector3Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        return new Vector3(
            Mathf.Clamp(value.x, min.x , max.x),
            Mathf.Clamp(value.y, min.y , max.y),
            Mathf.Clamp(value.z, min.z , max.z)
        );
    }

}
