using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    protected StateMachine stateMachine;

    public BaseState(StateMachine stateMachine){
        
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void Exit()
    {
        
    }
}
