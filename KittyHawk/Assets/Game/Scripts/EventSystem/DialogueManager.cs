using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class in charge of displaying and closing dialogue
/// Author: Calvin Ferst
/// </summary>
public class DialogueManager : MonoBehaviour
{

    [SerializeField]
    Canvas canvas;

    private bool displayingDialogue;
    private Dialogue currentDialogue;
    private int dialogueCount;
    private UnityAction<Vector3, string> dialogueEventListener;

    private TextMeshProUGUI speakerText;
    private TextMeshProUGUI dialogueText;

    private InputReader input;
    private bool subscribed;

    private string pathPrefix = "Dialogue";
    private Dictionary<string, Dialogue> dialogues;

    #region Setup

    void Start()
    {
        canvas.enabled = false;
        displayingDialogue = false;
        dialogueCount = 0;

        dialogues = new Dictionary<string, Dialogue>();

        input = GetComponent<InputReader>();

        FindTextDisplays();

        dialogueEventListener = new UnityAction<Vector3, string>(DisplayDialogue);
        EventManager.StartListening<DialogueOpenEvent, Vector3, string>(dialogueEventListener);
    }

    void FindTextDisplays()
    {
        TextMeshProUGUI[] textDisplays = canvas.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI textDisplay in textDisplays)
        {
            if (textDisplay.gameObject.name == "Speaker Text") speakerText = textDisplay;
            if (textDisplay.gameObject.name == "Dialogue Text") dialogueText = textDisplay;
        }

        if (speakerText == null || dialogueText == null) Debug.Log("Couldn't find necessary displays!");
    }

    void OnDestroy()
    {
        if (subscribed)
            input.JumpEvent -= UpdateDialogue;
        EventManager.StopListening<DialogueOpenEvent, Vector3, string>(dialogueEventListener);
    }

    #endregion

    #region Handling Dialogue

    public void DisplayDialogue(Vector3 position, string dialogueName)
    {
        if (!dialogues.TryGetValue(dialogueName, out Dialogue dialogue))
        {
            dialogue = LoadDialogue(dialogueName);
        }

        if (dialogue == null || dialogue.DialogueText.Length < 1)
        {
            Debug.Log("No dialogue found.");
            EventManager.TriggerEvent<DialogueCloseEvent, string>(null);
        }

        currentDialogue = dialogue;

        speakerText.text = currentDialogue.SpeakerName;
        dialogueText.text = currentDialogue.DialogueText[dialogueCount];

        canvas.enabled = true;
        displayingDialogue = true;
        input.JumpEvent += UpdateDialogue;
        subscribed = true;
    }

    void UpdateDialogue()
    {
        dialogueCount++;
        Debug.Log("DIALOGUE COUNT: " + dialogueCount);

        if (dialogueCount >= currentDialogue.DialogueText.Length)
        {
            canvas.enabled = false;
            displayingDialogue = false;
            dialogueCount = 0;
            input.JumpEvent -= UpdateDialogue;
            subscribed = false;
            EventManager.TriggerEvent<DialogueCloseEvent, string>(currentDialogue.DialogueName);
        }
        else
        {
            dialogueText.text = currentDialogue.DialogueText[dialogueCount];
        }
    }

    #endregion

    #region Helper

    Dialogue LoadDialogue(string name)
    {
        Dialogue dialogue = Resources.Load<Dialogue>(Path.Join(pathPrefix, name));
        dialogues.Add(name, dialogue);
        return dialogue;
    }

    #endregion


}
