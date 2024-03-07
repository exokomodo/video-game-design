using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class providing the bare structure of an action.
/// Author: Geoffrey Roth
/// </summary>
public abstract class StateAction
{
    public int ActionID;
    public int BlendingType;
    public abstract void Enter();
    public abstract void Execute(float deltaTime);
    public abstract void Exit();
}
