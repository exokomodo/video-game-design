using System;
using UnityEngine;

public class DuckDialogueTest : MonoBehaviour
{
    bool alreadyTalked = false;
    private InputReader input;
    public Canvas canvas;

    void Start()
    {
        input = GetComponent<InputReader>();
        if (input == null) throw new Exception("Input Reader could not be found");
        canvas.enabled = false;
        EventManager.StartListening<InteractionEvent, string, string, InteractionTarget>(OnInteractionEvent);
        EventManager.StartListening<DialogueOpenEvent, Vector3, string>(OnDialogOpen);
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogClose);
    }

    private void OnInteractionEvent(string eventName, string eventType, InteractionTarget target)
    {
        Debug.Log("DuckDialogueTest > InteractionEvent received " + eventName + ", " + eventType);
        switch (eventName)
        {
            case InteractionEvent.INTERACTION_ZONE_ENTERED:
                TogglePrompt(true);
                ToggleListeners(true);
                break;

            case InteractionEvent.INTERACTION_ZONE_EXITED:
                TogglePrompt(false);
                ToggleListeners(false);
                break;
        }
    }

    private void TogglePrompt(bool b)
    {
        canvas.enabled = b;
    }

    private void ToggleListeners(bool b)
    {
        if (b) {
            input.JumpEvent += DisplayDialogue;
            return;
        }
        input.JumpEvent -= DisplayDialogue;
    }

    private void OnDialogOpen(Vector3 position, string dialogueName)
    {
        Debug.Log("OnDialogOpen");
        TogglePrompt(false);
        ToggleListeners(false);
    }

    private void OnDialogClose(string dialogueName)
    {
        Debug.Log("OnDialogClose");
        TogglePrompt(true);
        ToggleListeners(true);
    }

    private void DisplayDialogue()
    {
        Debug.Log("DISPLAY: " + alreadyTalked);
        if (!alreadyTalked)
        {
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "TestDialogue");
            alreadyTalked = true;
        }
        else
        {
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "TestDialogue2");
        }
    }

    private void OnDestroy()
    {
        EventManager.StopListening<InteractionEvent, string, string, InteractionTarget>(OnInteractionEvent);
    }
}
