using UnityEngine;

public class CowBaseController : MonoBehaviour
{

    protected Animator anim;
    protected float talkCooldown = 10f;
    protected float timer;
    protected bool isTalking = false;
    protected string dialogueName = "";

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        timer = talkCooldown;
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogClose);
    }

    protected void OnTriggerEnter(Collider other)
    {
        // Allow Kitty to talk to cow again, but only after a 10 second cool down
        // in order to avoid accidental re-triggering of dialogue
        if (other.CompareTag("Player") && timer > talkCooldown)
        {
            anim.SetBool("jumping", true);
            anim.Play("Jumping");
            timer = 0;
            isTalking = true;
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, dialogueName);
        }
    }

    protected void Update() {
        if (!isTalking) timer += Time.deltaTime;
    }

    protected void OnDialogClose(string name) {
        // OnTriggerExit was being immediately called when the dialogue opened
        // This caused the cow to stop jumping (I think we wanted the cow to keep jumping during the dialogue).
        if (name == dialogueName) {
            isTalking = false;
            anim.SetBool("jumping", false);
        }
    }

    protected void OnDestroy() {
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogClose);
    }

}
