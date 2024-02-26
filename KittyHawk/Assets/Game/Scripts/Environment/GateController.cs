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

    // Update is called once per frame
    
    void Update()
    {
        // Takes user input to open gate for now
        // TODO: Open gate on key collectable 
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
