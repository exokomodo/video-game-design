using UnityEngine;
using UnityEngine.SceneManagement;


// Author Ben Lee
// Date: 2024
// Description: This script is used to trigger dialogue when the player enters the trigger zone of the cow. It also triggers a follow up dialogue if the player talks to the cow again.


public class CowFollow : MonoBehaviour
{

    bool alreadyTalked = false;
    public Canvas canvas;
    public Animator animator;
    public GameObject player;

    public string sceneName;

    private void OnTriggerEnter(Collider other) {
        if (other == player.GetComponent<Collider>() && !alreadyTalked) {
            Debug.Log("KITTY HAWK TRIGGERING DIALOGUE");

            // Added to get the current scene name so that the cow can have a different dialogue in Paul's level to tell kitty what to do 
            // Tried this in a Start() method but the script loaded before the scene so I called it here.
            sceneName = SceneManager.GetActiveScene().name; 
            animator.SetBool("jumping", true);
            if (!alreadyTalked) {
                // Added to trigger different dialogue in Paul's level
                if (sceneName == "PaulScene") {
                    EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowDialoguePaul");
                }

                // Original implementation
                else {
                EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowDialoguePaul");
                }

                alreadyTalked = true;
            } else {
                int randomInt = Random.Range(1, 4);
                EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowFollowupDialogue" + randomInt);
            }
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other == player.GetComponent<Collider>()) {
            animator.SetBool("jumping", false);
        }
    }


}
