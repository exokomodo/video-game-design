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
        if (Vector3.Distance(player.transform.position, transform.position) < 1.0f && Vector3.Angle(player.transform.forward, pond.transform.position - player.transform.position) < 45)
        {
            // log it to console
            Debug.Log("Player swats ball into pond");
            // EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "ScarecrowRustle");
            GetComponent<Rigidbody>().AddForce((pond.transform.position - transform.position+heightModifier).normalized*0.3f, ForceMode.Impulse);
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
