using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// LevelEvent
/// Author: Geoffrey Roth
/// </summary>
public class LevelEvent<T>: UnityEvent<string, T>
{
    public const string END_ROOM_ENTERED = "EndRoomEntered";
    public const string BUNNY_COLLECTED = "BunnyCollected";
    public const string BUNNY_DEPOSITED = "BunnyDeposited";
}
