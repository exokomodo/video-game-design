using UnityEngine;

/// <summary>
/// FSM using state singletons
/// Adapted from https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d
/// Author: Geoffrey Roth
/// </summary>
abstract public class FSMState<T>
{
  protected int StateChangeDHash = Animator.StringToHash("StateChange");
  protected int StateIDHash = Animator.StringToHash("StateID");
  abstract public void Enter (T entity);
  abstract public void Execute (T entity);
  abstract public void Exit(T entity);
}
