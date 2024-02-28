using System.Linq;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public PlayerController Controller { get; private set; }
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public float FreeMovementSpeed { get; private set; }
    [field: SerializeField] public float RunningSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
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

    public bool isJumping = false;
    public bool isRunning = false;
    private int[] motionStates = {(int) StateEnum.JUMP, (int) StateEnum.MOVE};
    private int StateChangeIDHash = Animator.StringToHash("StateChange");

    protected AnimationClip[] clips;

    private void Start()
    {
        MainCameraTransform = Camera.main.transform;
        // AddAnimationEndEvent();
        SwitchState(new PlayerIdleState(this));
    }

    public override void SwitchState(State newState)
    {
        int? previousSateID = previousState?.StateID;
        base.SwitchState(newState);
        Animator.SetInteger(StateIDHash, newState.StateID);
        Animator.SetBool(StateChangeIDHash, true);
        if (!previousSateID.Equals(null)) Animator.SetInteger(PrevStateIDHash, (int) previousSateID);
    }

    protected override void Update()
    {
        Animator.SetBool(StateChangeIDHash, false);
        if (!motionStates.Contains(currentState.StateID) && InputReader.MovementValue != Vector2.zero) {
            Debug.Log("Switch to PlayerMoveState");
            SwitchState(new PlayerMoveState(this));
            return;
        }
        currentState?.Execute(Time.deltaTime);
    }

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
}
