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

    public LayerMask groundMask;

    public bool fly = false;
    public bool inWater = false;

    public Animator animator;
    bool isWaiting = false;

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

        if(isWaiting){
            setJump = false;
        }

        if(setJump){
            animator.SetTrigger("jump");
            isWaiting = true;
            StopAllCoroutines();
            StartCoroutine(ResetWaiting());
        }


        animator.SetFloat("speed", playerInput.MovementInput.magnitude);
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

        if(isWaiting){
            setJump = false;
        }

        if(setJump){
            animator.SetTrigger("jump");
            isWaiting = true;
            StopAllCoroutines();
            StartCoroutine(ResetWaiting());
        }


        animator.SetFloat("speed", playerInput.MovementInput.magnitude);
        playerMovement.HandleWaterGravity(playerInput.IsJumping);
        playerMovement.WaterWalk(playerInput.MovementInput, playerInput.RunningPressed);

    }

    IEnumerator ResetWaiting(){

        yield return new WaitForSeconds(0.1f);
        animator.ResetTrigger("jump");
        isWaiting = false;

    }

    private void HandleMouseClick(){

        Ray playerRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        RaycastHit hit;

        if(Physics.Raycast(playerRay, out hit, interactionRayLength, groundMask)){

            VoxelType hitBlock = CheckTerrain(hit);

            if(!VoxelDataManager.voxelTextureDataDictionary[hitBlock].isDestructable){
                return;
            }
            
            bool modifiedTerrain = ModifyTerrain(hit);
            
            if(!modifiedTerrain){
                Debug.Log(hitBlock);
            }

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
