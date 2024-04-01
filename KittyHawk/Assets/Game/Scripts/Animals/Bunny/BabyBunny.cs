using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A Baby Bunny Controller.
/// Author: Geoffrey Roth
/// </summary>
public class BabyBunny : Bunny {

    protected bool collected = false;
    protected bool withMomma = false;

    public bool isEnabled = true;

    protected void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !collected) {
            Debug.Log("OnTriggerEnter");
            EventManager.TriggerEvent<LevelEvent<BabyBunny>, string, BabyBunny>(LevelEvent<BabyBunny>.BUNNY_COLLECTED, this);
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "success1");
            Follow(other.gameObject);
            GetComponent<BoxCollider>().enabled = false;
            collected = true;
        }
        // if (other.CompareTag("Finish") && !withMomma) {
        //     withMomma = true;
        //     followTarget = Waypoints[0]; // Momma
        //     lookAt = true;
        //     ChangeState(BunnyCelebrateState.Instance);
        // }
    }

    protected override void FixedUpdate() {
        // if (verticalVelocity < 0f && CheckGrounded()) {
        //     verticalVelocity = Physics.gravity.y * Time.fixedDeltaTime;
        // } else {
        //     verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
        // }
        // float velx = anim.GetFloat(VelocityXHash);
        // float velz = anim.GetFloat(VelocityZHash);

        // Vector3 newRootVelocity = new Vector3(velx * agent.speed, verticalVelocity, velz * agent.speed) + pendingMotion;
        // pendingMotion = Vector3.zero;
        // rb.velocity = newRootVelocity;
        FSM.Update();
    }

    public override void SetNextWaypoint() {
        if (!isEnabled) return;
        if (Waypoints.Count < 2 && collected) {
            // Quaternion rotation = transform.rotation;

            ChangeState(BunnyCelebrateState.Instance);
            return;
        }
        if (++currWaypoint >= Waypoints.Count) currWaypoint = 0;
        ChangeState(GetState());
    }

    public void Disable() {
        isEnabled = false;
        gameObject.SetActive(false);
    }
}
