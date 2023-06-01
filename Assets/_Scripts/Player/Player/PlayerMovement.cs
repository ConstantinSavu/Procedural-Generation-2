using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private float playerSpeed = 5.0f, playerRunSpeed = 8.0f;
    [SerializeField]
    private float playerWaterSpeed = 5.0f / 2f, playerWaterRunSpeed = 8.0f / 2f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float waterJumpHeight = 1.0f / 2f;

    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float waterGravityValue = -9.81f / 4f;

    [SerializeField]
    private float flySpeed = 5.0f, fastFlySpeed = 20.0f;

    private Vector3 playerVelocity;

    [Header("Check parameters")]
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float rayDistance = 0.1f;
    [field: SerializeField]
    public bool IsGrounded{get; private set;}

    private void Awake(){
        controller = GetComponent<CharacterController>();
    }

    private Vector3 GetMovemetDirection(Vector3 movementInput) {
        return transform.right * movementInput.normalized.x + transform.forward * movementInput.normalized.z;
    }

    public void Fly(Vector3 movementInput, bool goDownInput, bool goUpInput, bool isRunningInput){

        Vector3 movementDirection = GetMovemetDirection(movementInput);

        float currentSpeed = isRunningInput ? fastFlySpeed : flySpeed;

        if(goDownInput){
            movementDirection -= Vector3.up * flySpeed;
        }
        else if(goUpInput){
            movementDirection += Vector3.up * flySpeed;
        }

        controller.Move(movementDirection * currentSpeed * Time.deltaTime);

    }

    public void Walk(Vector3 movementInput, bool isRunningInput){

        Vector3 movementDirection = GetMovemetDirection(movementInput);

        float currentSpeed = isRunningInput ? playerRunSpeed : playerSpeed;

        controller.Move(movementDirection * currentSpeed * Time.deltaTime);

    }

    public void WaterWalk(Vector3 movementInput, bool isRunningInput)
    {
        Vector3 movementDirection = GetMovemetDirection(movementInput);

        float currentSpeed = isRunningInput ? playerWaterRunSpeed : playerWaterSpeed;

        controller.Move(movementDirection * currentSpeed * Time.deltaTime);
    }

    public void HandleGravity(bool IsJumping){

        if(IsGrounded && playerVelocity.y < 0){
            playerVelocity.y = 0f;
            
        }

        if(IsJumping && IsGrounded){
            AddJumpForce();
        }

        ApplyGravityForce();
        controller.Move(playerVelocity * Time.deltaTime);

    }

    public void HandleWaterGravity(bool IsJumping){

        if(IsGrounded){
            playerVelocity.y = 0.1f;
        }

        if(IsJumping){
            AddWaterJumpForce();
        }

        ApplyWaterGravityForce();
        controller.Move(playerVelocity * Time.deltaTime);

    }

    private void AddWaterJumpForce(){

        playerVelocity.y = waterJumpHeight;

    }

    private void AddJumpForce(){

        playerVelocity.y = jumpHeight;

    }

    private void ApplyGravityForce(){

        playerVelocity.y += gravityValue * Time.deltaTime;

    }

    private void ApplyWaterGravityForce(){

        playerVelocity.y += waterGravityValue * Time.deltaTime;

    }

    private void Update(){
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance, groundMask);
    }

    private void OnDrawGizmos(){
        Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
    }

    
}
