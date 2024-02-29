using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{

    public GameObject player;
    public GameObject gate;

    private float playerDistance;
    public float playerDistanceToActivate = 5.0f;
    private Animator animGate;
    private Animator animKey;
    private GateController gateController;


    // Start is called before the first frame update
    void Start()
    {
        animGate = gate.GetComponent<Animator>();
        animKey = GetComponent<Animator>();
        gateController = gate.GetComponent<GateController>();
    }

    public bool DetermineNearby()
    {
        // uses a vector to determine how far the player is, if the player is close enough, return true
        playerDistance = Vector3.Distance(player.transform.position, transform.position);
        return playerDistance < playerDistanceToActivate;

    }

    void FixedUpdate()
    {
        // If the player is close enough, the key will rotate,
        // if the player leaves, the key returns to its original position
        if (DetermineNearby())
        {
            animKey.SetBool("RotateKey", true);
        }
        else
        {
            animKey.SetBool("RotateKey", false);
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            gateController.setGateOpen();
            Destroy(this.gameObject);
        }
    }
}


