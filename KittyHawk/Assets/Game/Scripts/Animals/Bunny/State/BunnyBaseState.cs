using UnityEngine;

/// <summary>
/// A base state for the Bunny FSM using singletons
/// Author: Geoffrey Roth
/// </summary>
public abstract class BunnyBaseState : FSMState<Bunny> {

    protected Bunny bunny;
    protected GameObject target;

    protected int MagnitudeHash = Animator.StringToHash("Magnitude");
    protected int VelocityXHash = Animator.StringToHash("VelocityX");
    protected int VelocityZHash = Animator.StringToHash("VelocityZ");

    public override void Enter(Bunny b)
    {
        bunny = b;
        target = b.followTarget;
    }

    protected void SwitchAnimState(Bunny b, int StateID)
    {
        int curAnimStateID = b.anim.GetInteger(StateIDHash);
        if (curAnimStateID == StateID) return;
        b.anim.SetInteger(StateIDHash, StateID);
        b.anim.SetTrigger(StateChangeDHash);
    }

    protected float TargetHorizontalDistance(Bunny b)
    {
        Vector2 va = new Vector2(b.transform.position.x, b.transform.position.z);
        Vector2 vb = new Vector2(target.transform.position.x, target.transform.position.z);
        return Vector2.Distance(va, vb);
    }

}


