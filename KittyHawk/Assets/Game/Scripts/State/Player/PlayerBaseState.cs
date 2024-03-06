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

    protected void Rotate(Quaternion newRotation, float deltaTime)
    {
        Quaternion sourceRotation = stateMachine.Animator.rootRotation;
        stateMachine.Controller.Rotate(Quaternion.LerpUnclamped(sourceRotation, newRotation, deltaTime));
    }

    protected void Rotate(Vector3 sourcePosition, Vector3 targetPosition, float deltaTime)
    {
        Quaternion newRotation = Quaternion.FromToRotation(sourcePosition, targetPosition);
        newRotation.z = 0;
        stateMachine.Controller.Rotate(Quaternion.LerpUnclamped(stateMachine.Controller.transform.rotation, newRotation, deltaTime));
    }

    protected void Jump(float deltaTime)
    {
        stateMachine.Controller.Move(stateMachine.ForceReceiver.Movement * deltaTime);
    }

    protected void AddForce(Vector3 movement, ForceMode mode = ForceMode.Impulse)
    {
        stateMachine.Controller.AddForce(movement, mode);
    }
}
