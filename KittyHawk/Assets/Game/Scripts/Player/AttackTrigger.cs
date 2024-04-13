using UnityEngine;

/// <summary>
/// AttackTrigger: A trigger collider for Kitty that emits events
/// Author: Geoffrey Roth
/// </summary>
public class AttackTrigger : MonoBehaviour {

    [SerializeField]
    protected PlayerController Controller;
    protected float HitTimer;
    protected float MaxTimer = 1f;

    protected void Awake() {
        HitTimer = MaxTimer;
    }

    protected void FixedUpdate() {
        HitTimer += Time.fixedDeltaTime;
    }

    protected void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Bunny")) {
            EventManager.TriggerEvent<LevelEvent<Collider>, string, Collider>(LevelEvent<Room>.BUNNY_COLLIDER_ENTERED, c);
            return;
        }
        if (c.CompareTag("Goose")) OnGooseHit(c);
    }

    protected void OnTriggerStay(Collider c) {
        if (c.CompareTag("Goose")) OnGooseHit(c);
    }

    protected void OnTriggerExit(Collider c) {
        if (c.CompareTag("Goose")) {
            HitTimer = 0;
        }
    }

    protected void OnGooseHit(Collider c) {
        // Debug.Log($"OnGooseHit > isAttacking: {Controller.isAttacking}");
        if (Controller.isAttacking || Controller.isJumpAttacking) {
            if (HitTimer > MaxTimer) {
                // Debug.Log($"OnGooseHit > ATTACK_TARGET_HIT {c}");
                EventManager.TriggerEvent<AttackEvent, string, float, Collider>(AttackEvent.ATTACK_TARGET_HIT, 0f, c);
                HitTimer = 0;
            }
        }
    }
}
