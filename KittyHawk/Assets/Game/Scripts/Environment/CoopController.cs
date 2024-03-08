using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopController : MonoBehaviour
{

    private int chickenCounter = 0;
    public GameObject[] chicks;
    public int chicksToWin;
    public bool winnerWinnerChickenDinner;
    
    // Start is called before the first frame update
    void Start()
    {
        //TODO: Implement array and collect animators to play animation of walking in coop while 
        //coop door swings open
        //chicksToWin = chicks.Length;
        winnerWinnerChickenDinner = false;
    }

    public void checkWinStatus()
    {
        if (chickenCounter >= chicksToWin)
        {
            winnerWinnerChickenDinner = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!winnerWinnerChickenDinner) return;
        {
            Debug.Log("YOU WIN!");
            //TODO: Insert UI commands here?
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // On collision with chicken increases the chicken counter and checks if it was enough to win.
        if (other.gameObject.CompareTag("Chick"))
        {
            chickenCounter++;
            checkWinStatus();
        }
    }
}
