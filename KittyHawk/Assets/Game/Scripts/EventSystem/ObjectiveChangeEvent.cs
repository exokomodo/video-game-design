using UnityEngine.Events;

/// <summary>
/// Objective event to indicate change of objective status
/// Author: Calvin Ferst
/// </summary>
public class ObjectiveChangeEvent : UnityEvent<string, ObjectiveStatus> { }
