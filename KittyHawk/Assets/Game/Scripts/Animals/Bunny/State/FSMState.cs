using UnityEngine;

abstract public class FSMState<T>
{
  protected int StateChangeDHash = Animator.StringToHash("StateChange");
  protected int StateIDHash = Animator.StringToHash("StateID");
  protected int MagnitudeHash = Animator.StringToHash("Magnitude");
  protected int VelocityXHash = Animator.StringToHash("VelocityX");
  protected int VelocityZHash = Animator.StringToHash("VelocityZ");
  abstract public void Enter (T entity);
  abstract public void Execute (T entity);
  abstract public void Exit(T entity);
}
