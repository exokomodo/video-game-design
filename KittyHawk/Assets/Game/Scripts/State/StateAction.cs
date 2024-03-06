using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateAction
{
    public int ActionID;
    public int ActionType;
    public abstract void Enter();
    public abstract void Execute(float deltaTime);
    public abstract void Exit();
}
