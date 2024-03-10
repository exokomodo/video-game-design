using UnityEngine;

public class CowFollow : MonoBehaviour
{

    // Author Ben Lee
    // Date: 2024

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
