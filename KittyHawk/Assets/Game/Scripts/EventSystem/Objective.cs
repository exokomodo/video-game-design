using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject for creating objective assets
/// Author: Calvin Ferst
/// </summary>

public enum ObjectiveStatus { NotStarted, InProgress, Failed, Completed }

[CreateAssetMenu(fileName = "New Objective", menuName = "Pentaclaw/Objective")]
public class Objective : ScriptableObject
{

    public string ObjectiveName;
    public ObjectiveStatus Status;    

}
