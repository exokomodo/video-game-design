using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteractState : PlayerBaseState
{
    protected readonly int MatchTargetHash = Animator.StringToHash("MatchTarget");
    protected Transform targetTransform;

    public PlayerInteractState(PlayerStateMachine stateMachine, Transform targetTransform) : base(stateMachine) {
        this.StateID = 6;
        this.targetTransform = targetTransform;

    }

    public override void Enter()
    {
        Debug.Log("PlayerInteractState Enter");
    }

    public override void Execute(float deltaTime)
    {
        Vector3 currentPosition = stateMachine.Animator.rootPosition;
        Quaternion currentRotation = stateMachine.Animator.rootRotation;
        float dist = Vector3.Distance(currentPosition, targetTransform.position);
        float angle = Quaternion.Angle(currentRotation, targetTransform.rotation);
        bool needsMatchTarget = dist >= 1.0f || angle >= 10.0f;
        stateMachine.Animator.SetBool(MatchTargetHash, needsMatchTarget);
        // Debug.Log("needsMatchTarget: " + needsMatchTarget);
        if (needsMatchTarget)
        {

            Vector3 deltaVector = targetTransform.position - currentPosition;
            Move(deltaVector, deltaTime);
            Quaternion rot = Quaternion.LookRotation(new Vector3(deltaVector.x, 0, deltaVector.z));
            Rotate(rot, deltaTime);
        }

    }

    public override void Exit()
    {
        Debug.Log("PlayerInteractState Exit");
    }
}
