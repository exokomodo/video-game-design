using UnityEngine;

/// <summary>
/// Attach AttackEventBehavior to a state to specify attack begin and end interval
/// Author: Geoffrey Roth
/// </summary>
public class AttackEventBehavior : StateMachineBehaviour
{
    [Tooltip("Range on the Animation that the Attack Trigger will be Active")]
    public float attackBeginTime;
    public float attackEndTime;

    bool beginSent;
    bool endSent;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        beginSent = false;
        endSent = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        float stateTime = stateInfo.normalizedTime;
        if (!beginSent)
        {
            if (stateTime >= attackBeginTime)
            {
                beginSent = true;
                Trigger(AttackEvent.ATTACK_BEGIN, attackBeginTime);
            }
        }
        if (!endSent)
        {
            if (stateTime >= attackEndTime)
            {
                endSent = true;
                Trigger(AttackEvent.ATTACK_END, attackEndTime);
            }
        }
    }

    private void Trigger(string type, float value)
    {
        EventManager.TriggerEvent<AttackEvent, string, float, Collider>(type, value, null);
    }
}
