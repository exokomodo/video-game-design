/*
 * GateController.cs
 * Authors: Paul Garza
 * Date: 03/09/24
 * Summary: This script serves as the controller for the gate object in the fenced area of the farm.
 * Currently pressing 'o' on the keyboard opens the gate manually, but currently a key is required to open it.
 * 
 * Planned Updates:
 *
 * Dependencies: KeyController.cs will be able to change gateOpen.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator anim;
    public bool gateOpen = false;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void setGateOpen()
    {
        gateOpen = true;
    }
    // Update is called once per frame
    
    void Update()
    {
        // Takes user input to open gate for now
        if (Input.GetKeyDown("o"))
        {
            gateOpen = true;
        }
        // Changes the state of the gate to GateOpen from GateClosed
        if (gateOpen)
        {
            anim.Play("GateOpen");
        }
    }
}
