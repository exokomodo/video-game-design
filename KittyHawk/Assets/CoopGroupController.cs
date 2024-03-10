/*
 * CoopGroupController.cs
 * Authors: Paul Garza
 * Date: 03/09/24
 * Summary: This script serves as the controller for a group of children chicken coops. The children chicken coops
 * communicate via the addChicken() and checkForWin() functions, and the coop group keeps track of the
 * win condition.
 *
 * 
 * Planned updates: This script can still be tweaked some to self-define the chicksToWin variable, which is
 * currently necessary to set manually.
 *
 * Dependencies: ChickenCoop children components
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopGroupController : MonoBehaviour
{
    [SerializeField] private int totalChickens = 0;
    public int chicksToWin;
    public bool winnerWinnerChickenDinner;
    
    // Start is called before the first frame update
    void Start()
    {
        // TODO: Automatically set chicks to win or just decide how many it will be
        totalChickens = 0;
        winnerWinnerChickenDinner = false;
    }
    
    public void addChicken()
    {
        totalChickens++;
    }

    public void checkForWin()
    {
        if (totalChickens == chicksToWin)
        {
            winnerWinnerChickenDinner = true;
            UnityEngine.Debug.Log("Chickens collected! You win!");
        }
    }

}
