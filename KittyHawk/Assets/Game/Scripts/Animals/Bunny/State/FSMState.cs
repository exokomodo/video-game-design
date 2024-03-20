using UnityEngine;

abstract public class FSMState<T>
{
  protected int StateChangeDHash = Animator.StringToHash("StateChange");
  protected int StateIDHash = Animator.StringToHash("StateID");
  abstract public void Enter (T entity);
  abstract public void Execute (T entity);
  abstract public void Exit(T entity);
}
