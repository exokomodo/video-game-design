using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A Baby Bunny Controller.
/// Author: Geoffrey Roth
/// </summary>
public class BabyBunny : Bunny {

    protected void Start() {

        EventManager.StartListening<LevelEvent<Collider>, string, Collider>(OnLevelEvent);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("OnTriggerEnter");
            EventManager.TriggerEvent<LevelEvent<BabyBunny>, string, BabyBunny>(LevelEvent<BabyBunny>.BUNNY_COLLECTED, this);
            Follow(other.gameObject);
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    protected void OnLevelEvent(string eventType, Collider c) {
        Debug.Log("OnLevelEvent<Collider>: " + c);
        switch (eventType) {
            case LevelEvent<Collider>.END_ROOM_ENTERED:
                if (followMode == true) {
                    GameObject go = GameObject.FindWithTag("Bunny");
                    currWaypoint = 0;
                    Waypoints = new List<GameObject>{go};
                    Patrol();
                }
                break;
        }
    }

    protected void OnDestroy() {
        EventManager.StopListening<LevelEvent<Collider>, string, Collider>(OnLevelEvent);
    }
}
