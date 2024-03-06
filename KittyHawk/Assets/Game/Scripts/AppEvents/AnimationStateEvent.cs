using UnityEngine.Events;

public class AnimationStateEvent: UnityEvent<AnimationStateEventBehavior.AnimationEventType, string> {

    public const string ATTACK_COMPLETE = "AttackComplete";
    public const string JUMP_COMPLETE = "JumpComplete";

    public const string LAND_BEGIN = "LandBegin";
    public const string LAND_COMPLETE = "LandComplete";

    public const string MEOW = "Meow";
}
