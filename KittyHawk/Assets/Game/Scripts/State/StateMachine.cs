using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State currentState;
    private State previousState;

    public void SwitchState(State newState)
    {
        previousState = currentState;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    private void Update()
    {
        currentState?.Execute(Time.deltaTime);
    }

    public void RevertState()
    {
        if (previousState != null)
        {
            SwitchState(previousState);
        }
    }
}
