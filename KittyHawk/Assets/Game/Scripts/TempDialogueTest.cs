using UnityEngine;

public class TempDialogueTest : MonoBehaviour
{

    bool alreadyTalked = false;

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            Debug.Log("KITTY HAWK TRIGGERING DIALOGUE");

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
    }
}
