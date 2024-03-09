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
