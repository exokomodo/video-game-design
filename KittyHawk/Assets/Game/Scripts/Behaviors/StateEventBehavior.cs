using UnityEngine;
using UnityEngine.Animations;

public class StateEventBehavior : StateMachineBehaviour
{
    [Tooltip("Enter Event Name")]
    public string EnterEvent = "StateEnter";

    [Tooltip("Exit Event Name")]
    public string ExitEvent = "StateExit";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("StateEventBehavior: OnStateEnter - " + EnterEvent);
    }

    public void OStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("StateEventBehavior: OStateExit - " + ExitEvent);
    }
}
