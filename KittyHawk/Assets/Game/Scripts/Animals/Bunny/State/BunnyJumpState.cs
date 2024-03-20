using System;
using UnityEngine;

public sealed class BunnyJumpState : BunnyBaseState {
    private bool isJumping;
    private float elapsedTime = 0;
    static readonly BunnyJumpState instance = new BunnyJumpState();
    public static BunnyJumpState Instance {
        get {
            return instance;
        }
    }
    private bool agentUpdate;

    private Vector3 momentum;
    static BunnyJumpState() {}
    private BunnyJumpState() {}

    public override void Enter(Bunny b)
    {
        Debug.Log("Enter BunnyJumpState");
        base.Enter(b);
        elapsedTime = 0;
        momentum = agentUpdate? bunny.agent.velocity : bunny.velocity;
        agentUpdate = b.UpdateAgent;
        b.UpdateAgent = false;
        isJumping = false;
        AnimatorStateInfo info = b.anim.GetCurrentAnimatorStateInfo(-1);
        b.anim.Play(info.fullPathHash, -1, 10);
        EventManager.StartListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
    }

    public override void Execute(Bunny b) {
        b.anim.speed = 1;
        if (!isJumping && b.isGrounded && elapsedTime > 0.1f)
        {
            Debug.Log($"MOMENTUM: {momentum}");
            SwitchAnimState(b, (int)Bunny.BunnyAnimState.JUMP);
            isJumping = true;

            b.Move(momentum + new Vector3(0, 6f, 0));
            // b.Jump(50f);

        }
        else
        {
            b.anim.SetFloat(VelocityXHash, momentum.x);
            b.anim.SetFloat(VelocityZHash, momentum.z);
            b.anim.SetFloat(MagnitudeHash, momentum.magnitude);
        }
        elapsedTime += Time.fixedDeltaTime;
    }

    public override void Exit(Bunny b) {
        Debug.Log("Leaving BunnyJumpState");
        b.UpdateAgent = agentUpdate;
        EventManager.StopListening<AnimationStateEvent, AnimationStateEventBehavior.AnimationEventType, string>(OnAnimationEvent);
    }



    private void OnAnimationEvent(AnimationStateEventBehavior.AnimationEventType eventType, string eventName)
  {
    Debug.Log("Bunny AnimationEvent received " + eventType + ", " + eventName);
    switch (eventName)
    {
      case AnimationStateEvent.BUNNY_JUMP_COMPLETE:
        bunny.ChangeState(BunnyFollowState.Instance);
        break;
    }
  }
}
