using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Bunny Events
/// Author: Geoffrey Roth
/// </summary>
public class BunnyEvent: UnityEvent<string, float, Collider>
{
    public const string Bunny_BEGIN = "BunnyBegin";
    public const string Bunny_END = "BunnyEnd";
    public const string Bunny_TARGET_HIT = "BunnyTargetHit";

    public const string Bunny_STATE_ENTER = "BunnyStateEnter";
    public const string Bunny_STATE_EXIT = "BunnyStateExit";
}
