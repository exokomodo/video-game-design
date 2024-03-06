using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopController : MonoBehaviour
{

    private int chickenCounter = 0;
    private int chicksToWin = 1;
    public bool winnerWinnerChickenDinner = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (winnerWinnerChickenDinner) return;
        
        if (chickenCounter >= chicksToWin)
        {
            winnerWinnerChickenDinner = true;
            Debug.Log("YOUWIN!");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Chick"))
        {
            chickenCounter++;
        }
    }
}
