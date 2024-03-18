using UnityEngine;

/// <summary>
/// The abstract base class for all Kitty actions
/// Corresponding animations are found in the "ActionsLayer" of the Animation Controller.
/// Author: Geoffrey Roth
/// </summary>
public abstract class PlayerBaseAction : StateAction
{
    protected PlayerStateMachine stateMachine;
    protected static readonly string ActionLayer = "ActionLayer";
    protected static readonly string HeadLayer = "HeadLayer";
    protected readonly int ActionIDHash = Animator.StringToHash("ActionID");
    protected readonly int ActionLayerHash = Animator.StringToHash("ActionLayer");
    public PlayerBaseAction(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        // Actions may blend via override (default) or additive
        BlendingType = (int)PlayerStateMachine.BlendingType.OVERRIDE;
    }
}
