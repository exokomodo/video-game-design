using System;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// A Baby Bunny Controller.
/// Author: Geoffrey Roth
/// </summary>
public class BabyBunny : Bunny {


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("OnTriggerEnter");
            Follow(other.gameObject);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
