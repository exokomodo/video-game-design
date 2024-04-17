/*
 * KeyController.cs
 * Authors: Paul Garza, James Orson
 * Date: 03/09/24
 * Summary: This script serves as the controller for a collectable key. Right now, collecting it will
 * open a gate on the farm.
 *
 * Planned updates: This script can still be altered to apply to more than just the gate.
 *
 * Dependencies: Something to open, int his case the gate.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private GameObject player;
    private GameObject gate;
    private float playerDistance;
    private Animator animGate;
    private Animator animKey;
    private GateController gateController;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        animKey = GetComponent<Animator>();

        gate = GameObject.FindWithTag("Gate");
        gateController = gate.GetComponent<GateController>();
        animGate = gate.GetComponent<Animator>();
    }

    // Pickup key and open gate.
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gateController.setGateOpen();
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "success1");
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>("Objective_Key", ObjectiveStatus.Completed);
            Destroy(this.gameObject);
        }

    }

    // Animates key based on if kitty is in the sphere collider or not
    private void OnTriggerEnter(Collider c)
    {
        animKey.SetBool("RotateKey", true);
    }

    private void OnTriggerExit(Collider other)
    {
        animKey.SetBool("RotateKey", false);
    }
}


