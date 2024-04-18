using UnityEngine;

public class CowDuckController : MonoBehaviour
{

    private Animator anim;
    private float talkCooldown = 5f;
    private float timer;
    private bool isTalking = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        timer = talkCooldown;
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogClose);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Allow Kitty to talk to cow again, but only after a 5 second cool down
        // in order to avoid accidental re-triggering of dialogue
        if (other.CompareTag("Player") && timer > talkCooldown)
        {
            anim.SetBool("jumping", true);
            anim.Play("Jumping");
            timer = 0;
            isTalking = true;
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowDuckDialogue");
        }
    }

    private void Update() {
        if (!isTalking) timer += Time.deltaTime;
    }

    private void OnDialogClose(string name) {
        // OnTriggerExit was being immediately called when the dialogue opened
        // This caused the cow to stop jumping (I think we wanted the cow to keep jumping during the dialogue).
        if (name == "CowDuckDialogue") {
            isTalking = false;
            anim.SetBool("jumping", false);
        }
    }

    private void OnDestroy() {
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogClose);
    }

}
