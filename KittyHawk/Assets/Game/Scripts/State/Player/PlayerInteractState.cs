using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteractState : PlayerBaseState
{
    protected readonly int MatchTargetHash = Animator.StringToHash("MatchTarget");
    protected readonly int isRunningHash = Animator.StringToHash("isRunning");
    protected Transform targetTransform;
    protected Bounds targetBounds;

    protected readonly Vector3 interactionOffset = new Vector3(1, 0, 0);

    public PlayerInteractState(PlayerStateMachine stateMachine, Transform targetTransform, Bounds targetBounds) : base(stateMachine) {
        this.StateID = 6;
        this.targetTransform = targetTransform;
        this.targetBounds = targetBounds;
    }

    public override void Enter()
    {
        Debug.Log("PlayerInteractState Enter");
    }

    public override void Execute(float deltaTime)
    {
        Vector3 currentPosition = stateMachine.Controller.headPosition;
        currentPosition.y = 0;
        Vector3 targetPosition = targetTransform.position;
        targetPosition.y = 0;
        float dist = Vector3.Distance(currentPosition, targetPosition);
        bool needsMatchTarget = dist >= targetBounds.size.x + 0.01f;
        stateMachine.Animator.SetBool(MatchTargetHash, needsMatchTarget);
        stateMachine.Animator.SetBool(isRunningHash, false);
        if (needsMatchTarget)
        {
            // Debug.Log("dist: " + dist + ", angle: " + angle);
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
