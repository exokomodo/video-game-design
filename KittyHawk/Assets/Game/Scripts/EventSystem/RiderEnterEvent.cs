using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// RiderEnterEvent represents an event for when a rider collides with a rideable object
/// Author: James Orson
/// </summary>

public class RiderEnterEvent : UnityEvent<Rideable, GameObject> { }
