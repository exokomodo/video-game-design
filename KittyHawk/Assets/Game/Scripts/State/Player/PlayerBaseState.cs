using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;

    protected int StateIDHash = Animator.StringToHash("StateID");

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(Vector3 movement, float deltaTime)
    {
        stateMachine.Controller.Move((movement + stateMachine.ForceReceiver.Movement) * deltaTime);
    }

    protected void AddForce(Vector3 movement, ForceMode mode = ForceMode.Impulse)
    {
        stateMachine.Controller.AddForce(movement, mode);
    }
}
