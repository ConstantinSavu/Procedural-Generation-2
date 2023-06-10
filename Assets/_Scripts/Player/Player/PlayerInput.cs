using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action OnMouseLeftClick, OnMouseRightClick, OnFly;
    public bool RunningPressed{get; private set;}
    public bool CrouchingingPressed{get; private set;}
    public bool IsJumping{get; private set;}
    public Vector3 MovementInput{get; private set;}
    public Vector2 MousePosition{get; private set;}



    void Update(){
        GetMouseLeftClick();
        GetMouseRightClick();
        GetMousePosition();
        GetMovementInput();
        GetJumpInput();
        GetRunInput();
        GetCrouchInput();
        GetFlyInput();

        if(Input.GetKeyDown(KeyCode.P)){
            if(Time.timeScale == 1){
               Time.timeScale = 0; 
            }
            else{
                Time.timeScale = 1;
            }
        }
    }

    private void GetFlyInput(){
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.F)){
            OnFly?.Invoke();
        }
        #endif
    }

    private void GetRunInput(){
        RunningPressed = Input.GetKey(KeyCode.LeftShift);
    }

    private void GetCrouchInput(){
        CrouchingingPressed = Input.GetKey(KeyCode.LeftControl);
    }

    private void GetJumpInput(){
        IsJumping = Input.GetButton("Jump");
    }

    private void GetMovementInput(){

        MovementInput = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

    }

    private void GetMousePosition(){
        MousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void GetMouseLeftClick(){

        if(Input.GetMouseButtonDown(0)){
            OnMouseLeftClick?.Invoke();
        }

    }

    private void GetMouseRightClick(){

        if(Input.GetMouseButtonDown(1)){
            OnMouseRightClick?.Invoke();
        }

    }
}
