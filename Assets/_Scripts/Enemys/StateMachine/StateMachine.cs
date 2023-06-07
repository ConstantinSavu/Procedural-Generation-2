using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private BaseState currentState;
    public BaseState CurrentState{get; protected set;}
    void Start()
    {
        CurrentState = GetInitialState();
        if(CurrentState != null){
            CurrentState.Enter();
        }
    }
    void Update()
    {
        if(CurrentState != null){
            CurrentState.Update();
        }
    }

    void FixedUpdate()
    {
        if(CurrentState != null){
            CurrentState.FixedUpdate();
        }
    }

    public void ChangeState(BaseState newState){
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    protected virtual BaseState GetInitialState(){
        return null;
    }

    
}
