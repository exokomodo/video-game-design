using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Pentaclaw/Dialogue")]
public class Dialogue : ScriptableObject
{

    public string DialogueName;
    public string SpeakerName;

    [TextArea(1, 5)]
    public string[] DialogueText;

}
