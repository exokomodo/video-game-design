using UnityEngine.Events;

/// <summary>
/// AnimationStateEvents are generally triggered by AnimationStateEventBehaviors
/// attached to Animation Controller states
/// Author: Geoffrey Roth
/// </summary>
public class AnimationStateEvent: UnityEvent<AnimationStateEventBehavior.AnimationEventType, string>
{
    public const string INTERACTION_COMPLETE = "InteractionComplete";
    public const string JUMP_COMPLETE = "JumpComplete";
    public const string LAND_BEGIN = "LandBegin";
    public const string LAND_COMPLETE = "LandComplete";
    public const string MEOW = "Meow";
    public const string MEOW_COMPLETE = "MeowComplete";
}
