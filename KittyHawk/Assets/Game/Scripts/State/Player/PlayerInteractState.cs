using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    protected readonly int MatchTargetHash = Animator.StringToHash("MatchTarget");
    protected readonly int InteractionIDHash = Animator.StringToHash("InteractionID");
    protected Transform targetTransform;
    protected Bounds targetBounds;
    protected static readonly Dictionary<string, int> InteractionTypeMap = new Dictionary<string, int>
    {
        { InteractionType.INTERACTION_BUTTON_PRESS, (int)PlayerStateMachine.InteractionEnum.BUTTON_PRESS },
        { InteractionType.INTERACTION_ITEM_PICKUP, (int)PlayerStateMachine.InteractionEnum.ITEM_PICKUP },
        { InteractionType.INTERACTION_ITEM_DROP, (int)PlayerStateMachine.InteractionEnum.ITEM_DROP },
        { InteractionType.INTERACTION_ITEM_THROW, (int)PlayerStateMachine.InteractionEnum.ITEM_THROW },
    };
    public int InteractionID {get; private set;}

    public PlayerInteractState(PlayerStateMachine stateMachine, string eventType, Transform targetTransform, Bounds targetBounds) : base(stateMachine) {
        StateID = (int)PlayerStateMachine.StateEnum.INTERACT;
        this.targetTransform = targetTransform;
        this.targetBounds = targetBounds;
        this.InteractionID = InteractionTypeMap[eventType];
    }

    public override void Enter()
    {
        Debug.Log("PlayerInteractState Enter");
        stateMachine.Controller.ToggleRunning(false);
        stateMachine.Animator.SetInteger(InteractionIDHash, InteractionID);
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
        stateMachine.Animator.SetInteger(InteractionIDHash, -1);
    }
}
