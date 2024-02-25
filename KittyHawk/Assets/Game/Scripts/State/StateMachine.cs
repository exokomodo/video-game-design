using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State currentState;
    protected State previousState;

    public void SwitchState(State newState)
    {
        previousState = currentState;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    protected abstract void Update();

    public void RevertState()
    {
        if (previousState != null)
        {
            SwitchState(previousState);
        }
    }
}
