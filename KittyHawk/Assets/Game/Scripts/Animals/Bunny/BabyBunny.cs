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
            EventManager.TriggerEvent<LevelEvent<BabyBunny>, string, BabyBunny>(LevelEvent<BabyBunny>.BUNNY_COLLECTED, this);
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "success1");
            Follow(other.gameObject);
            GetComponent<BoxCollider>().enabled = false;
            collected = true;
        }
    }

    protected override void FixedUpdate() {
        FSM.Update();
    }

    public override void SetNextWaypoint() {
        if (!isEnabled) return;
        if (Waypoints.Count < 2 && collected) {
            ChangeState(BunnyCelebrateState.Instance);
            return;
        }
        if (++currWaypoint >= Waypoints.Count) currWaypoint = 0;
        ChangeState(GetState());
    }

    public void Enable() {
        isEnabled = true;
        gameObject.SetActive(true);
    }

    public void Disable() {
        isEnabled = false;
        gameObject.SetActive(false);
    }
}
