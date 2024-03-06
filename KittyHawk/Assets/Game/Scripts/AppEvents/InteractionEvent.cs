using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionEvent: UnityEvent<string, Transform>{
    public const string INTERACTION_AVAILABLE = "InteractionAvailable";
    public const string INTERACTION_TRIGGERED = "InteractionTriggered";
}
