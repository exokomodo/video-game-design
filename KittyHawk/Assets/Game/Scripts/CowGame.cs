using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


    // Author Ben Lee
    // Date: 2024
    // Description: This script is used to trigger dialogue when the player enters the trigger zone of the cow. It also triggers a follow up dialogue if the player talks to the cow again.
    // After the initial dialogue, the balls will be launched at random angles and the player will have swat them into the pond. The player will have 60 seconds to swat 5 balls.

public class CowGame: MonoBehaviour
{



    bool alreadyTalked = false;
    public Canvas canvas;
    public Animator animator;
    public GameObject player;
    public GameObject ball;
    public GameObject spawn;
    public Canvas minigameCanvas;

    public Canvas inventoryCanvas;
    private int score = 0;
    private float timeLeft = 60.0f;

    private Vector3 heightModifier;
    private Vector3 leftRightModifier;

    public GameObject timeDisplay;
    public GameObject scoreDisplay;


    private void Start() {
           // hide inventory canvas
           minigameCanvas.enabled = false;
           Vector3 heightModifier = new Vector3(0, 10, 0);
    }
    private void LaunchBall() {
        // Launch a new ball at random force towards object (transform of cow)

        GameObject ballObject = Instantiate(ball, spawn.transform.position, Quaternion.identity);

        Rigidbody ballRb = ballObject.GetComponent<Rigidbody>();

        // Calculate the direction of the ball
        Vector3 direction = transform.position - ballObject.transform.position;
        Vector3 leftRightModifier = new Vector3(Random.Range(-10, 10), 0, 0);
        direction += heightModifier + leftRightModifier;
        // Calculate the force
        float force = Random.Range(4, 6);
        // Launch the ball
        ballRb.AddForce(direction.normalized * force, ForceMode.Impulse);
        ballRb.useGravity = true;
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "tire-stack-bounce");
    }

    public void UpdateScore() {
        score++;
        scoreDisplay.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    private void OnTriggerEnter(Collider other) {
        if (other == player.GetComponent<Collider>() && !alreadyTalked) {
            Debug.Log("KITTY HAWK TRIGGERING DIALOGUE");
            animator.SetBool("jumping", true);
            if (!alreadyTalked) {
                EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowScene");
                EventManager.StartListening<DialogueCloseEvent, string>(OnDialogueFinished);
            } else {
                EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowSceneFollowupDialogue");
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other == player.GetComponent<Collider>()) {
            animator.SetBool("jumping", false);
        }
    }

    private void OnDialogueFinished(string dialogueName) {
        // enable canvas
        minigameCanvas.enabled = true;
        inventoryCanvas.enabled = false;
        alreadyTalked = true;
        EventManager.TriggerEvent<MusicEvent, string>("Kitty Polka");
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>("CowObjective", ObjectiveStatus.InProgress);
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogueFinished);
    }

    private void FixedUpdate() {

        if (alreadyTalked) {
            timeLeft -= Time.deltaTime;
            timeDisplay.GetComponent<TextMeshProUGUI>().text = timeLeft.ToString("F0")+" sec";
            if (timeLeft < 0) {
                // end the game
                alreadyTalked = false;
                if (score >= 5) {
                    EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "success-fanfare-trumpets");
                    EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>("CowObjective", ObjectiveStatus.Completed);
                }
                else {
                // restart scene
                    EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "CatHit1");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }

            }
            // randomly determine if a ball should be launched
            if (Random.Range(0, 200) == 1) {
                LaunchBall();
            }
        }
    }

}
