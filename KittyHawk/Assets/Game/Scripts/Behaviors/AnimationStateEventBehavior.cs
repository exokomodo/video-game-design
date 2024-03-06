using UnityEngine;
using System;


public class AnimationStateEventBehavior : StateMachineBehaviour
{

    public string enterEventValue;
    public string exitEventValue;
    public string timeEventValue;
    public float sendTime;

    public enum AnimationEventType
    {
        ENTER,
        EXIT,
        TIME
    }

    private bool isValidTimeEvent;
    private bool timeSent;

    public AnimationStateEventBehavior()
    {
        isValidTimeEvent = !sendTime.Equals(null) && timeEventValue != String.Empty;
        timeSent = false;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        timeSent = false;
        Trigger(AnimationEventType.ENTER, enterEventValue);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        Trigger(AnimationEventType.EXIT, exitEventValue);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (isValidTimeEvent)
        {
            float stateTime = stateInfo.normalizedTime;
            if (!timeSent)
            {
                if (stateTime >= sendTime)
                {
                    timeSent = true;
                    Trigger(AnimationEventType.TIME, timeEventValue);
                }
            }
        }
    }

    private void Trigger(AnimationEventType type, string value)
    {
        if (value != String.Empty) EventManager.TriggerEvent<AnimationStateEvent, AnimationEventType, string>(type, value);
    }
}

