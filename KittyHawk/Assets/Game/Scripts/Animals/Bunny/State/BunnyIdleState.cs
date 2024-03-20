using UnityEngine;

public sealed class BunnyIdleState : BunnyBaseState
{
    private const float THRESHOLD = 1.25f;
    static readonly BunnyIdleState instance = new BunnyIdleState();
    public static BunnyIdleState Instance
    {
        get {
            return instance;
        }
    }

    static BunnyIdleState() {}
    private BunnyIdleState() {}

    public override void Enter(Bunny b)
    {
        Debug.Log("Enter BunnyIdleState");
        base.Enter(b);

        bunny = b;
        SwitchAnimState(b, (int)Bunny.BunnyAnimState.IDLE);
        b.anim.speed = 1.4f;
        b.UpdateAgent = false;
        // b.anim.SetFloat(VelocityXHash, 0);
        // b.anim.SetFloat(VelocityZHash, 0);
        // b.anim.SetFloat(MagnitudeHash, 0);
    }

    public override void Execute(Bunny b)
    {
        b.anim.SetFloat(VelocityXHash, 0);
        b.anim.SetFloat(VelocityZHash, 0);
        b.anim.SetFloat(MagnitudeHash, 0);
        // b.UpdateAgent = false;
        // float velx = b.anim.GetFloat(VelocityXHash);
        // float velz = b.anim.GetFloat(VelocityZHash);
        // Vector3 newVector = Vector3.Lerp(new Vector3(velx, 0, velz), Vector3.zero, Time.fixedDeltaTime);
        // b.anim.SetFloat(VelocityXHash, newVector.x);
        // b.anim.SetFloat(VelocityZHash, newVector.z);
        // b.anim.SetFloat(MagnitudeHash, Mathf.Sqrt((float)Math.Pow(newVector.x, 2) + (float)Math.Pow(newVector.z, 2)));

        if (b.followMode && TargetHorizontalDistance(b) >= 2 * THRESHOLD)
        {
            b.agent.enabled = true;
            b.UpdateAgent = true;
            b.ChangeState(BunnyFollowState.Instance);
        }
    }

    public override void Exit(Bunny b) {
        Debug.Log("Exit BunnyIdleState");
    }
}
