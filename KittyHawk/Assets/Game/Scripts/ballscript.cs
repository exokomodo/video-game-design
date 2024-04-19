using System.Runtime.CompilerServices;
using UnityEngine;

// Author Ben Lee
// Date: April 2024
// Description: this script handles the ball swatting mechanic in the game. The player must swat the balls into the pond to score points. The player has 60 seconds to swat 5 balls.

public class ballscript : MonoBehaviour
{
    public GameObject pond;

    public GameObject player;
    public GameObject cow;
    Vector3 heightModifier = new Vector3(0, 5, 0);

    private float timeSinceLastMeow;

    private float meowThreshold = 30.0f;



    public Canvas canvas;

    PlayerController playerController;
    void Start()
    {
        // get the player object
        PlayerController playerController = player.GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // player touching ball and facing pond?
        // if (Vector3.Distance(player.transform.position, transform.position) < 1.0f && Vector3.Angle(player.transform.forward, pond.transform.position - player.transform.position) < 45)
        if (Vector3.Distance(player.transform.position, transform.position) < 0.4f)
        {
            // log it to console
            Debug.Log("Player swats ball into pond");
            timeSinceLastMeow += Time.deltaTime;

            if (timeSinceLastMeow > meowThreshold)
            {
                timeSinceLastMeow = 0;
                EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "CatMeow");
            }

            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "CatAttack3");
            // if player is facing the pond force is stronger. A little bit of assistance to help the player
            if  (Vector3.Angle(player.transform.forward, pond.transform.position - player.transform.position) < 45) {   
                GetComponent<Rigidbody>().AddForce((pond.transform.position - transform.position + heightModifier).normalized * 0.4f, ForceMode.Impulse);
            } 
            else {
                GetComponent<Rigidbody>().AddForce((player.transform.forward).normalized * 0.1f, ForceMode.Impulse);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if ball enters pond collider
        if (other.gameObject == pond)
        {
            // log it to console
            Debug.Log("Ball entered pond");
            // destroy the ball
            Destroy(gameObject);
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "success1");
            cow.GetComponent<CowGame>().UpdateScore();

        }
    }
}
