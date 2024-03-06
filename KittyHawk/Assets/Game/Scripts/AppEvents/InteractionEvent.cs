using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionEvent: UnityEvent<string, string, Transform, Bounds>{
    public const string INTERACTION_ZONE_ENTERED = "InteractionZoneEntered";
    public const string INTERACTION_ZONE_EXITED = "InteractionZoneExited";
    public const string INTERACTION_TRIGGERED = "InteractionTriggered";
}
public class InteractionType
{
    public const string INTERACTION_BUTTON_PRESS = "InteractionButtonPress";
    public const string INTERACTION_ITEM_PICKUP = "InteractionItemPickup";
    public const string INTERACTION_ITEM_DROP = "InteractionItemDrop";
    public const string INTERACTION_ITEM_THROW = "InteractionItemThrow";
}
