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
        // Quaternion oldRotation = stateMachine.Controller.transform.rotation;
        Quaternion oldRotation = stateMachine.Animator.rootRotation;
        stateMachine.Controller.Rotate(Quaternion.LerpUnclamped(oldRotation, newRotation, deltaTime));
    }

    protected void Rotate(Vector3 positionA, Vector3 positionB, float deltaTime)
    {
        Quaternion newRotation = Quaternion.FromToRotation(positionA, positionB);
        newRotation.z = 0;
        stateMachine.Controller.Rotate(Quaternion.LerpUnclamped(stateMachine.Controller.transform.rotation, newRotation, deltaTime));
        // stateMachine.Controller.Rotate(newRotation);
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
