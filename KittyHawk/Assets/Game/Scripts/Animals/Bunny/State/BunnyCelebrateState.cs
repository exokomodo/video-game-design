using UnityEngine;

/// <summary>
/// Celebrate state for the Bunny FSM using singletons
/// This has been replaced with the AgentLinkMover
/// Author: Geoffrey Roth
/// </summary>
public sealed class BunnyCelebrateState : BunnyBaseState {
    private bool isJumping;
    private float elapsedTime = 0;
    static readonly BunnyCelebrateState instance = new BunnyCelebrateState();
    public static BunnyCelebrateState Instance {
        get {
            return instance;
        }
    }
    private bool agentUpdate;

    private Vector3 momentum;
    static BunnyCelebrateState() {}
    private BunnyCelebrateState() {}

    public override void Enter(Bunny b)
    {
        Debug.Log("Enter BunnyCelebrateState");
        base.Enter(b);
        // b.agent.enabled = false;
        // b.UpdateAgent = false;

        elapsedTime = 0;
        // float velx = b.anim.GetFloat(VelocityXHash);
        // float velz = b.anim.GetFloat(VelocityZHash);
        // momentum = new Vector3(velx, 6f, velz);

        isJumping = false;
        // AnimatorStateInfo info = b.anim.GetCurrentAnimatorStateInfo(-1);
        // b.anim.Play(info.fullPathHash, -1, 12);
        EventManager.StartListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
    }

    public override void Execute(Bunny b) {
        b.anim.speed = 1.4f;
        if (!isJumping && elapsedTime > 0.1f)
        {
            // Debug.Log($"MOMENTUM: {momentum}");
            // b.agent.enabled = false;
            b.UpdateAgent = false;
            SwitchAnimState(b, (int)Bunny.BunnyAnimState.CELEBRATE);
            isJumping = true;
            // b.Move(momentum);
        }
        else
        {
            // b.anim.SetFloat(VelocityXHash, momentum.x);
            // b.anim.SetFloat(VelocityZHash, momentum.z);
            // b.anim.SetFloat(MagnitudeHash, momentum.magnitude);
        }
        elapsedTime += Time.fixedDeltaTime;
    }

    public override void Exit(Bunny b) {
        // Debug.Log("Leaving BunnyCelebrateState");
        // b.Move(Vector3.zero);
        // b.UpdateAgent = false;
        EventManager.StopListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
    }



    private void OnAnimationEvent(AnimationStateEventBehavior.AnimationEventType eventType, string eventName)
    {
        // Debug.Log("Bunny AnimationEvent received " + eventType + ", " + eventName);
        switch (eventName)
        {
            case AnimationStateEvent.BUNNY_JUMP_COMPLETE:
                // bunny.ChangeState(BunnyCelebrateState.Instance);
                break;
        }
    }
}
