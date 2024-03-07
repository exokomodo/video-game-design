using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class providing the bare structure of a state.
/// Author: Geoffrey Roth
/// </summary>
public abstract class State
{
    public int StateID;
    public abstract void Enter();
    public abstract void Execute(float deltaTime);
    public abstract void Exit();
}
