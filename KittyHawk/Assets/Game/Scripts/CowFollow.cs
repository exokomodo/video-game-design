using UnityEngine;


    // Author Ben Lee
    // Date: 2024
    // Description: This script is used to trigger dialogue when the player enters the trigger zone of the cow. It also triggers a follow up dialogue if the player talks to the cow again.
    
public class CowFollow : MonoBehaviour
{



    bool alreadyTalked = false;
    public Canvas canvas;
    public Animator animator;
    public GameObject player;

    private void OnTriggerEnter(Collider other) {
        if (other == player.GetComponent<Collider>() && !alreadyTalked) {
            Debug.Log("KITTY HAWK TRIGGERING DIALOGUE");
            animator.SetBool("jumping", true);
            if (!alreadyTalked) {
                EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowDialogue");
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
