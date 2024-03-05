using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionEvent: UnityEvent<string, Vector3>{
    public const string INTERACTION_AVAILABLE = "InteractionAvailable";
    public const string INTERACTION_TRIGGERED = "InteractionTriggered";
}
