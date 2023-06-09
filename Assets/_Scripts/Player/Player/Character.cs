using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    public PlayerCamera playerCamera;

    [SerializeField] int attackDamage = 1;


    

    public World world;

    public float interactionRayLength = 5;

    public LayerMask hitMask;

    public LayerMask digMask;

    public LayerMask enemyMask;
    public bool fly = false;
    public bool inWater = false;

    public Animator animator;
    bool isJumpWaiting = false;

    public bool isAttacking = false;
    private float animationFinnishTime = 0.9f;

    private float runningModifier = 0.5f;


    public void Awake(){

        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponentInChildren<PlayerCamera>();
        world = FindObjectOfType<World>();
       

    }

    private void Start(){
        playerInput.OnMouseLeftClick += HandleLeftMouseClick;
        
        playerInput.OnFly += HandleFlyClick;
    }

    private void HandleFlyClick(){
        fly = !fly;
    }

    void Update(){

        if(PauseManager.gameIsPaused){
            return;
        }

        if(isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinnishTime){
            isAttacking = false;
        }

        if(fly){

            animator.SetFloat("speed", 0);
            animator.SetBool("isGrounded", false);
            animator.ResetTrigger("jump");
            playerMovement.Fly(playerInput.MovementInput, playerInput.CrouchingingPressed, playerInput.IsJumping, playerInput.RunningPressed);
            return;
        }

        CheckIfInWater();

        if(inWater){
            WaterMovement();
            return;
        }

        
        SolidMovement();
        
            
    }

    void CheckIfInWater()
    {

        if(world == null){
            return;
        }

        VoxelType voxelType = world.CheckVoxel(transform.position);

        if(voxelType == VoxelType.Water){
            
            inWater = true;
            
        }
        else{
            
            inWater = false;
        }

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

    private void HandleLeftMouseClick(){
        if(PauseManager.gameIsPaused){
            return;
        }

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
        
        Ray playerRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        RaycastHit hit;


        if(Physics.Raycast(playerRay, out hit, interactionRayLength, hitMask)){
            
            int hitLayer = (1 << hit.transform.gameObject.layer);
            Debug.Log("Hit something");
            if((hitLayer | digMask.value) == digMask.value){
                TerrainHit(hit);
            }
            else if((hitLayer | enemyMask.value) == enemyMask.value){
                EnemyHit(hit);
            }
        
        }
        else{
           Debug.Log("No hit something"); 
        }
        
    }

    private void EnemyHit(RaycastHit hit){
        Debug.Log("Hit Enemy");
        hit.transform.GetComponent<EnemyHealthSystem>().TakeDamage(attackDamage);
    }

    private void TerrainHit(RaycastHit hit){

        Vector3Int pos;
        ChunkRenderer chunk;

        VoxelType hitBlock = CheckTerrain(hit, out pos, out chunk);
        

        if(!VoxelDataManager.voxelTextureDataDictionary[hitBlock].isDestructable){
            Debug.Log("Hit Indesctructable " + hitBlock);
            pos = new Vector3Int(-1, -1, -1);
            return;
        }
        
        
        bool modifiedTerrain = ModifyTerrain(pos, chunk);
        
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

    private bool ModifyTerrain(Vector3Int pos, ChunkRenderer chunk){
        
        if(world == null){
            return false;
        }
        
        
        return world.SetVoxel(pos, chunk, VoxelType.Air);

    }

    private VoxelType CheckTerrain(RaycastHit hit, out Vector3Int pos, out ChunkRenderer chunk){
        
        if(world == null){
            pos = new Vector3Int(-1, -1, -1);
            chunk = null;
            return VoxelType.Nothing;
        }

        return world.CheckVoxel(hit, out pos, out chunk);
    }

    

}
