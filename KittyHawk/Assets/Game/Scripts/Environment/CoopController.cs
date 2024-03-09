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
