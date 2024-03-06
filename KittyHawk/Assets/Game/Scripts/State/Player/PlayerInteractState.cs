using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteractState : PlayerBaseState
{
    protected readonly int MatchTargetHash = Animator.StringToHash("MatchTarget");
    protected Transform targetTransform;

    protected readonly Vector3 interactionOffset = new Vector3(1, 0, 0);

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
        // Vector3 currentPosition = stateMachine.Animator.rootPosition;
        Vector3 pos = stateMachine.Controller.headPosition;
        Vector3 currentPosition = new Vector3(pos.x, pos.y, pos.z);
        Quaternion currentRotation = stateMachine.Animator.rootRotation;
        float dist = Vector3.Distance(currentPosition, targetTransform.position);
        float angle = Quaternion.Angle(currentRotation, targetTransform.rotation);

        bool needsMatchTarget = dist >= 0.5f || angle >= 10.0f;
        stateMachine.Animator.SetBool(MatchTargetHash, needsMatchTarget);
        if (needsMatchTarget)
        {
            Vector3 deltaVector = targetTransform.position - currentPosition;
            Quaternion rot = Quaternion.LookRotation(new Vector3(deltaVector.x, 0, deltaVector.z));
            Move(deltaVector, deltaTime);
            Rotate(rot, deltaTime);
        }

    }

    public override void Exit()
    {
        Debug.Log("PlayerInteractState Exit");
    }
}
