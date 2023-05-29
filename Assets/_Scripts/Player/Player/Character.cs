using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private PlayerMovement playerMovement;

    

    public World world;

    public float interactionRayLength = 5;

    public LayerMask hitMask;

    public LayerMask groundMask;

    public LayerMask enemyMask;
    public bool fly = false;
    public bool inWater = false;

    public Animator animator;
    bool isJumpWaiting = false;

    public bool isAttacking = false;
    private float animationFinnishTime = 0.9f;

    private float runningModifier = 0.5f;
    

    public void Awake(){
        
        if(mainCamera == null){
            mainCamera = Camera.main;
        }

        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();

        world = FindObjectOfType<World>();
       

    }

    private void Start(){
        playerInput.OnMouseClick += HandleMouseClick;
        playerInput.OnFly += HandleFlyClick;
    }

    private void HandleFlyClick(){
        fly = !fly;
    }

    public void InWater(){
        inWater = true;
    }

    public void OnSolid(){
        inWater = false;
    }

    void Update(){


        if(fly){

            animator.SetFloat("speed", 0);
            animator.SetBool("isGrounded", false);
            animator.ResetTrigger("jump");
            playerMovement.Fly(playerInput.MovementInput, playerInput.CrouchingingPressed, playerInput.IsJumping, playerInput.RunningPressed);
            return;
        }

        if(isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinnishTime){
            isAttacking = false;
        }

        if(inWater){
            WaterMovement();
            return;
        }

        
        SolidMovement();
        
            
    }

    private void SolidMovement()
    {
        animator.SetBool("isGrounded", playerMovement.IsGrounded);

        bool setJump = true;

        if(!playerMovement.IsGrounded){
            setJump = false;
        }

        if(!playerInput.IsJumping){
            setJump = false;
        }

        if(isJumpWaiting){
            setJump = false;
        }

        if(setJump){
            animator.SetTrigger("jump");
            isJumpWaiting = true;
            StopCoroutine(ResetJumpWaiting());
            StartCoroutine(ResetJumpWaiting());
        }

        int runningPressed = playerInput.RunningPressed ? 1 : 0;

        float animationSpeed = Math.Clamp(playerInput.MovementInput.magnitude, 0, 1) +
                                runningPressed * runningModifier;
        animator.SetFloat("speed", animationSpeed);
        playerMovement.HandleGravity(playerInput.IsJumping);
        playerMovement.Walk(playerInput.MovementInput, playerInput.RunningPressed);
    }

    private void WaterMovement()
    {   
        
        animator.SetBool("isGrounded", playerMovement.IsGrounded);

        bool setJump = true;

        if(!playerMovement.IsGrounded){
            setJump = false;
        }

        if(!playerInput.IsJumping){
            setJump = false;
        }

        if(isJumpWaiting){
            setJump = false;
        }

        if(setJump){
            animator.SetTrigger("jump");
            isJumpWaiting = true;
            StopCoroutine(ResetJumpWaiting());
            StartCoroutine(ResetJumpWaiting());
        }


        int runningPressed = playerInput.RunningPressed ? 1 : 0;

        float animationSpeed = Math.Clamp(playerInput.MovementInput.magnitude, 0, 1) +
                                runningPressed * runningModifier;
        animator.SetFloat("speed", animationSpeed);
        playerMovement.HandleWaterGravity(playerInput.IsJumping);
        playerMovement.WaterWalk(playerInput.MovementInput, playerInput.RunningPressed);

    }

    IEnumerator ResetJumpWaiting(){

        yield return new WaitForSeconds(0.1f);
        animator.ResetTrigger("jump");
        isJumpWaiting = false;

    }

    IEnumerator ResetAttackWaiting(){

        yield return new WaitForSeconds(0.1f);
        isAttacking = true;

    }

    private void HandleMouseClick(){


        if(!isAttacking){
            Attack();
        }
        else{   
            Debug.Log("Can't attack");
        }
        

    }

    private void Attack(){
        
        animator.SetTrigger("isAttacking");
        StopCoroutine(ResetAttackWaiting());
        StartCoroutine(ResetAttackWaiting());
        
        Ray playerRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        RaycastHit hit;


        if(Physics.Raycast(playerRay, out hit, interactionRayLength, hitMask)){

            LayerMask hitLayer = (1 << hit.transform.gameObject.layer);

            if(hitLayer == groundMask.value){
                TerrainHit(hit);
            }
            else if(hitLayer == enemyMask.value){
                EnemyHit(hit);
            }
                  

        }
        
    }
    private void EnemyHit(RaycastHit hit){
        Debug.Log("Hit Enemy");
        hit.transform.GetComponent<NavMeshEnemyMovement>().TakeDamage(1f);
    }

    private void TerrainHit(RaycastHit hit){

        VoxelType hitBlock = CheckTerrain(hit);
            try{

                if(!VoxelDataManager.voxelTextureDataDictionary[hitBlock].isDestructable){
                    return;
                }
            }
            catch(Exception e){
                Debug.Log(e.Message);
            }
            
            bool modifiedTerrain = ModifyTerrain(hit);
            
            if(!modifiedTerrain){
                Debug.Log(hitBlock);
            }

    }

    private bool ModifyTerrain(RaycastHit hit){
        
        if(world == null){
            return false;
        }
        
        
        return world.SetVoxel(hit, VoxelType.Air);

    }

    private VoxelType CheckTerrain(RaycastHit hit){
        
        if(world == null){
            return VoxelType.Nothing;
        }

        return world.CheckVoxel(hit);
    }

    

}
