
using UnityEngine;


/// <summary>
/// Basic script handle return visits
/// Author: Geoffrey Roth
/// </summary>
public class DungeonDuckController : SmokingDuckController {
    protected float talkCooldown = 10f;
    protected float timer;
    protected bool isTalking = false;

    protected override void Awake() {
        base.Awake();
        timer = talkCooldown;
        ToggleTalking(true);
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogClose);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && timer > talkCooldown)
        {
            ToggleTalking(true);
            timer = 0;
            isTalking = true;
            string dialogueName = $"BunnyFollowUpDialogue{Random.Range(1, 5)}";
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, dialogueName);
        }
    }

    protected void Update() {
        if (!isTalking) timer += Time.deltaTime;
    }

    protected void OnDialogClose(string name) {
        isTalking = false;
        ToggleTalking(false);
    }

    protected void OnDestroy() {
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogClose);
    }
}
