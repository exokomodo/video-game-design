using System.Linq;
using UnityEngine;

/// <summary>
/// The state machine for Kitty. Managers her states and blended actions.
/// Author: Geoffrey Roth
/// </summary>
public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public PlayerController Controller { get; private set; }
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }

    public enum StateEnum
    {
        IDLE,
        SIT,
        LIE,
        MOVE,
        JUMP,
        FALL,
        INTERACT,
        CRAWL,
        SWIM,
        DIE,
        TURN
    }

    public enum InteractionEnum
    {
        BUTTON_PRESS,
        DIG,
        ITEM_DROP,
        ITEM_PICKUP,
        ITEM_THROW
    }

    public enum ActionEnum
    {
        ATTACK_RIGHT,
        ATTACK_FRONT,
        ATTACK_LEFT,
        MEOW,
        HIT
    }

    public enum BlendingType
    {
        OVERRIDE,
        ADDITIVE
    }

    public Transform MainCameraTransform { get; private set; }

    public bool isIdle => currentState?.StateID == (int) StateEnum.IDLE;
    public bool isSitting => currentState?.StateID == (int) StateEnum.SIT;
    public bool isLaying => currentState?.StateID == (int) StateEnum.LIE;
    public bool isJumping => currentState?.StateID == (int) StateEnum.JUMP;
    public bool isRunning => Controller.isRunning;

    public bool isMoving => currentState?.StateID == (int) StateEnum.MOVE;
    public bool isFalling => currentState?.StateID == (int) StateEnum.FALL;
    public bool isCrawling => currentState?.StateID == (int) StateEnum.CRAWL;

    private int[] motionStates = {(int) StateEnum.JUMP, (int) StateEnum.MOVE, (int)StateEnum.FALL, (int)StateEnum.TURN};
    [HideInInspector]
    public int[] idleStates = {(int) StateEnum.IDLE, (int) StateEnum.SIT, (int)StateEnum.LIE};
    private int StateChangeHash = Animator.StringToHash("StateChange");

    protected AnimationClip[] clips;

    private bool firstUpdate;

    private void Start()
    {
        MainCameraTransform = Camera.main.transform;
        // AddAnimationEndEvent();
        SwitchState(new PlayerIdleState(this));
    }

    public override void SwitchState(State newState)
    {
        previousState = currentState == null? newState : currentState;
        int? previousSateID = previousState?.StateID;
        currentState?.Exit();

        currentState = newState;
        currentState?.Enter();
        // Animator.SetInteger(StateIDHash, currentState.StateID);
        firstUpdate = true;


        if (!previousSateID.Equals(null)) Animator.SetInteger(PrevStateIDHash, (int) previousSateID);
    }

    protected override void Update()
    {
        if (Animator.updateMode == AnimatorUpdateMode.AnimatePhysics) return;
        Execute(Time.deltaTime);
    }

    protected override void FixedUpdate()
    {
        if (Animator.updateMode != AnimatorUpdateMode.AnimatePhysics) return;
        Execute(Time.fixedDeltaTime);
    }

    protected override void Execute(float deltaTime)
    {
        if (!motionStates.Contains(currentState.StateID) &&
            InputReader.MovementValue != Vector2.zero &&
            Controller.isActive
        ) {
            SwitchState(new PlayerMoveState(this));
            return;
        }
        currentState?.Execute(deltaTime);
        if (firstUpdate && currentState != null)
        {
            Animator.SetInteger(StateIDHash, currentState.StateID);
            Animator.SetBool(StateChangeHash, true);
            firstUpdate = false;
        }

        currentAction?.Execute(deltaTime);
        additiveAction?.Execute(deltaTime);
    }

    public override void SwitchAction(StateAction newAction)
    {
        Debug.Log("SwitchAction: " + newAction);
        if (newAction.BlendingType == (int)BlendingType.ADDITIVE)
        {
            if (additiveAction != null) return;
            additiveAction = newAction;
            additiveAction.Enter();
            return;
        }
        if (currentAction != null) return;
        previousAction = currentAction == null? newAction : currentAction;
        currentAction?.Exit();
        currentAction = newAction;
        currentAction?.Enter();
        Animator.SetInteger(ActionIDHash, currentAction.ActionID);
        Animator.SetBool(ActionChangeHash, true);
    }

    public void ActionComplete(PlayerBaseAction action)
    {
        // Debug.Log($"ACTION COMPLETE {action.BlendingType}");
        action.Exit();
        if (action.BlendingType == (int)BlendingType.ADDITIVE)
        {
            additiveAction = null;
            return;
        }
        Animator.SetInteger(ActionIDHash, -1);
        currentAction = null;
    }
}
