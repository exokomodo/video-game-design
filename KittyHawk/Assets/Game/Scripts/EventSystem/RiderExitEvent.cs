using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// RiderExitEvent represents an event for when a rider leaves a rideable object
/// Author: James Orson
/// </summary>

public class RiderExitEvent : UnityEvent<Rideable, GameObject> { }
