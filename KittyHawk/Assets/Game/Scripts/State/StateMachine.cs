using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State currentState;
    public State CurrentState => currentState;
    protected State previousState;
    protected int StateIDHash = Animator.StringToHash("StateID");
    protected int PrevStateIDHash = Animator.StringToHash("PrevStateID");

    public virtual void SwitchState(State newState)
    {
        previousState = currentState;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
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
