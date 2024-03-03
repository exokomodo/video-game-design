using System.Linq;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public PlayerController Controller { get; private set; }
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }

    public enum StateEnum
    {
        IDLE,
        SIT,
        LIE,
        MOVE,
        JUMP,
        FALL,
        CLIMB,
        CRAWL,
        SWIM,
        ATTACK,
        DIE
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

    // Incomplete... add crawl, fall, climb, etc
    public int[] motionStates = {(int) StateEnum.JUMP, (int) StateEnum.MOVE, (int)StateEnum.FALL};
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
        if (Animator.GetBool(StateChangeHash))
            Animator.SetBool(StateChangeHash, false);
        if (!motionStates.Contains(currentState.StateID) && InputReader.MovementValue != Vector2.zero) {
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

        if (currentAction != null)
        {
            if (Animator.GetBool(ActionChangeHash))
                Animator.SetBool(ActionChangeHash, false);
            currentAction?.Execute(deltaTime);
        }

    }

    public override void SwitchAction(StateAction newAction)
    {
        Debug.Log("SwitchAction: " + newAction);
        previousAction = currentAction == null? newAction : currentAction;
        currentAction?.Exit();

        currentAction = newAction;
        currentAction?.Enter();
        Animator.SetInteger(ActionIDHash, currentAction.ActionID);
        Animator.SetBool(ActionChangeHash, true);
    }

    /*
    private void AddAnimationEndEvent()
    {
        clips = Animator.runtimeAnimatorController.animationClips;
        AnimationEvent evt = new AnimationEvent();
        Debug.Log("clips: " + clips);
        foreach(AnimationClip clip in clips)
        {
            Debug.Log("CLIP: " + clip.name + ", LENGTH: " + clip.length);
            evt.time = clip.length - 0.05f;
            evt.stringParameter = clip.name;
            evt.functionName = "OnClipEnd";
            clip.AddEvent(evt);
        }
    }

    public void OnClipEnd(string clipName)
    {
        Debug.Log("OnClipEnd: " + clipName + " called at: " + Time.time);
        // if (isJumping)
        // {
        //     SwitchState(new PlayerMoveState(this));
        //     isJumping = false;
        // }
    }
    */
}
