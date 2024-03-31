using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A Baby Bunny Controller.
/// Author: Geoffrey Roth
/// </summary>
public class BabyBunny : Bunny {

    protected void Start() {

        EventManager.StartListening<LevelEvent<Room>, string, Room>(OnLevelEvent);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("OnTriggerEnter");
            EventManager.TriggerEvent<LevelEvent<BabyBunny>, string, BabyBunny>(LevelEvent<BabyBunny>.BUNNY_COLLECTED, this);
            Follow(other.gameObject);
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    protected void OnLevelEvent(string eventType, Room room) {
        Debug.Log("OnLevelEvent<Room>: " + room);
        switch (eventType) {
            case LevelEvent<Room>.END_ROOM_ENTERED:
                if (followMode == true) {
                    GameObject go = GameObject.FindWithTag("Finish");
                    Waypoints = new List<GameObject>{go};

                    Patrol();
                }
                break;
        }
    }

    protected void OnDestroy() {
        EventManager.StopListening<LevelEvent<Room>, string, Room>(OnLevelEvent);
    }
}
