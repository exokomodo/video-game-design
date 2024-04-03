using UnityEngine;


    // Author Ben Lee
    // Date: 2024
    // Description: This script is used to trigger dialogue when the player enters the trigger zone of the cow. It also triggers a follow up dialogue if the player talks to the cow again.
    // After the initial dialogue, the balls will be launched at random angles and the player will have swat them into the pond. The player will have 60 seconds to swat 5 balls.
    
public class CowFollow : MonoBehaviour
{



    bool alreadyTalked = false;
    public Canvas canvas;
    public Animator animator;
    public GameObject player;

    public GameObject ball;

    public GameObject spawn;

    private int score = 0;

    private float timeLeft = 60.0f;

    private void LaunchBall() {
        // Launch a new ball at random force towards object (transform of cow)

        GameObject ballObject = Instantiate(ball, spawn.transform.position, Quaternion.identity);

        Rigidbody ballRb = ballObject.GetComponent<Rigidbody>();

        // Calculate the direction of the ball

        Vector3 heightModifier = new Vector3(0, 10, 0);
        Vector3 leftRightModifier = new Vector3(Random.Range(-10, 10), 0, 0);
        Vector3 direction = transform.position - ballObject.transform.position;
        direction += heightModifier + leftRightModifier;
 

        // Calculate the force
        float force = Random.Range(4, 6);

        // Launch the ball
        ballRb.AddForce(direction.normalized * force, ForceMode.Impulse);
        ballRb.useGravity = true;
        


        

      
    }

    private void UpdateScore() {
        // Update the score
    }

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

    private void FixedUpdate() {


        // randomly determine if a ball should be launched
        if (Random.Range(0, 200) == 1) {
            LaunchBall();
        }




        // handle score display
        
        // handle calls to launch ball

        // handle calls to update score

        // handle timer

        // listen for ball collisions with pond surface

    }

}
