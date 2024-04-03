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

    BoxCollider boxCollider;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void setGateOpen()
    {
        gateOpen = true;
        boxCollider.enabled = false;
    }
    // Update is called once per frame
    
    void Update()
    {
        // Changes the state of the gate to GateOpen from GateClosed
        if (gateOpen)
        {
            anim.Play("GateOpen");
        }
    }
}
