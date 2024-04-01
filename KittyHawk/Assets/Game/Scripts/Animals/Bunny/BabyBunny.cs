using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A Baby Bunny Controller.
/// Author: Geoffrey Roth
/// </summary>
public class BabyBunny : Bunny {

    protected bool collected = false;

    protected void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !collected) {
            Debug.Log("OnTriggerEnter");
            EventManager.TriggerEvent<LevelEvent<BabyBunny>, string, BabyBunny>(LevelEvent<BabyBunny>.BUNNY_COLLECTED, this);
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "success1");
            Follow(other.gameObject);
            GetComponent<BoxCollider>().enabled = false;
            collected = true;
        }
    }

    protected override void FixedUpdate() {
        if (verticalVelocity < 0f && CheckGrounded()) {
            verticalVelocity = Physics.gravity.y * Time.fixedDeltaTime;
        } else {
            verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
        }
        FSM.Update();
    }
}
