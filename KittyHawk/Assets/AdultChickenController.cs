using UnityEngine;

public class AdultChickenController : MonoBehaviour
{

    protected float talkCooldown = 10f;
    protected float timer;
    protected bool isTalking = false;
    protected string dialogueName = "ChickenDialogue";

    protected virtual void Start()
    {
        timer = talkCooldown;
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogClose);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && timer > talkCooldown)
        {
            timer = 0;
            isTalking = true;
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, dialogueName);
        }
    }

    protected void Update() {
        if (!isTalking) timer += Time.deltaTime;
    }

    protected void OnDialogClose(string name) {
        if (name == dialogueName) {
            isTalking = false;
        }
    }

    protected void OnDestroy() {
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogClose);
    }
}
