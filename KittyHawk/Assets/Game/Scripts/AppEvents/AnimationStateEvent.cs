using UnityEngine.Events;

public class AnimationStateEvent: UnityEvent<AnimationStateEventBehavior.AnimationEventType, string>{
    public const string LAND_COMPLETE = "LandComplete";
}
