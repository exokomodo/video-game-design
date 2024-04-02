/*
 * CoopGroupController.cs
 * Authors: Paul Garza
 * Date: 03/09/24
 * Summary: This script serves as the controller for a group of children chicken coops. The children chicken coops
 * communicate via the addChicken() and checkForWin() functions, and the coop group keeps track of the
 * win condition.
 *
 * Update: 03/10/24 added event manager trigger for objective complete.
 * 
 * Planned updates: This script can still be tweaked some to self-define the chicksToWin variable, which is
 * currently necessary to set manually.
 *
 * Dependencies: ChickenCoop children components
 */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoopGroupController : MonoBehaviour
{
    [SerializeField] private int totalCapturedChickens = 0;
    [SerializeField] private int chicksToWin;

    [SerializeField] PlayerController playerController;

    private PlayerInventory inventory;

    public bool winnerWinnerChickenDinner;

    public Text chickenCounter;

    public bool kittyNearCoops;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Chick");
        chicksToWin = objectsWithTag.Length;
        winnerWinnerChickenDinner = false;
        inventory = playerController.GetComponent<PlayerInventory>();
        inventory.Chickens = 0;
    }

    public void addChicken()
    {
        totalCapturedChickens++;
        inventory.Chickens = totalCapturedChickens;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            kittyNearCoops = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            kittyNearCoops = false;
        }
    }

    public void checkForWin()
    {
        if (totalCapturedChickens >= chicksToWin)
        {
            winnerWinnerChickenDinner = true;
            Debug.Log("Chicken Objective Completed");
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>("ChickObjective", ObjectiveStatus.Completed);
        }
    }

}
