using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// InterationEvent generally triggered by Interactable
/// Author: Geoffrey Roth
/// </summary>
public class InteractionEvent: UnityEvent<string, string, InteractionTarget>{
    public const string INTERACTION_ZONE_ENTERED = "InteractionZoneEntered";
    public const string INTERACTION_ZONE_EXITED = "InteractionZoneExited";
    public const string INTERACTION_TRIGGERED = "InteractionTriggered";
}
public class InteractionType
{
    public const string NONE = "None";
    public const string INTERACTION_BUTTON_PRESS = "InteractionButtonPress";
    public const string INTERACTION_DIALOGUE = "InteractionDialogue";
    public const string INTERACTION_DIG = "InteractionDig";
    public const string INTERACTION_ITEM_DROP = "InteractionItemDrop";
    public const string INTERACTION_ITEM_PICKUP = "InteractionItemPickup";
    public const string INTERACTION_ITEM_THROW = "InteractionItemThrow";
}
