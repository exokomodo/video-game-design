using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseAction : StateAction
{
    protected PlayerStateMachine stateMachine;
    protected static readonly string ActionLayer = "ActionLayer";
    protected readonly int ActionIDHash = Animator.StringToHash("ActionID");
    protected readonly int ActionLayerHash = Animator.StringToHash("ActionLayer");

    public PlayerBaseAction(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
}
