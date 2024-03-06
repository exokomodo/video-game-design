using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State currentState;
    protected State previousState;
    protected StateAction currentAction;
    protected StateAction previousAction;
    public StateAction CurrentAction => currentAction;
    public State CurrentState => currentState;
    public int CurrentStateID
    {
        get { return currentState != null? currentState.StateID : -1;}
    }
    public int CurrentActionID
    {
        get { return currentAction != null? currentAction.ActionID : -1; }
    }
    protected int ActionChangeHash = Animator.StringToHash("ActionChange");
    protected int ActionIDHash = Animator.StringToHash("ActionID");

    protected int StateIDHash = Animator.StringToHash("StateID");
    protected int PrevStateIDHash = Animator.StringToHash("PrevStateID");

    public virtual void SwitchState(State newState)
    {
        previousState = currentState;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public virtual void SwitchAction(StateAction newAction)
    {
        previousAction = currentAction;
        currentAction?.Exit();
        currentAction = newAction;
        currentAction?.Enter();
    }

    protected abstract void Update();
    protected abstract void FixedUpdate();
    protected abstract void Execute(float deltaTime);

    public void RevertState()
    {
        if (previousState != null)
        {
            SwitchState(previousState);
        }
    }
}
