/*
 * CoopController.cs
 * Authors: Paul Garza
 * Date: 03/09/24
 * Summary: This script serves as the controller for a single child chicken coops. These children chicken coops
 * communicate via the addChicken() and checkForWin() functions to CoopGroupController.cs,
 * and the coop group keeps track of the win condition.
 *
 * Planned updates: This script can still be tweaked to more realistically
 *
 * Dependencies: ChickenCoop group parent component
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopController : MonoBehaviour
{
    public GameObject parentCoopGroup;
    private CoopGroupController parentCoopGroupController;
    
    // Start is called before the first frame update
    void Start()
    {
        parentCoopGroupController = parentCoopGroup.GetComponent<CoopGroupController>();
        //TODO: Implement array and collect animators to play animation of walking in coop while 
        //coop door swings open
    }
    

    private void OnCollisionEnter(Collision other)
    {
        // On collision with chicken increases the chicken counter and checks if it was enough to win.
        if (other.gameObject.CompareTag("Chick"))
        {
            parentCoopGroupController.addChicken();
            parentCoopGroupController.checkForWin();
        }
    }
}
