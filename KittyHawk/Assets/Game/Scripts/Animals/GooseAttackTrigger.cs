using UnityEngine;

/// <summary>
/// GooseAttackTrigger: A trigger collider for Geese that emits events
/// Helps ensure that the goose is facing Kitty for an attack to be successful
/// Author: Geoffrey Roth
/// </summary>
public class GooseAttackTrigger : MonoBehaviour {

    [SerializeField]
    protected GooseAI Controller;
    protected float HitTimer;
    protected float MaxTimer = 5.0f;

    protected void Awake() {
        HitTimer = MaxTimer;
        // Controller = transform.root.GetComponent<GooseAI>();
        // Debug.Log("Controller: " + Controller);
    }

    protected void FixedUpdate() {
        HitTimer += Time.fixedDeltaTime;
    }

    protected void OnTriggerEnter(Collider c) {
        // Debug.Log("GooseAttackTrigger > OnTriggerEnter");
        if (c.CompareTag("Player")) OnKittyHit(c);
    }

    protected void OnTriggerStay(Collider c) {
        // Debug.Log("GooseAttackTrigger > OnTriggerStay");
        if (c.CompareTag("Player")) OnKittyHit(c);
    }

    protected void OnTriggerExit(Collider c) {
        // Debug.Log("GooseAttackTrigger > OnTriggerExit");
        if (c.CompareTag("Player")) {
            HitTimer = 0;
        }
    }

    protected void OnKittyHit(Collider c) {
        // Debug.Log($"GooseAttackTrigger > OnKittyHit > isAttacking: {Controller.IsAttacking}");
        if (Controller.IsAlive && Controller.IsAttacking) {
            // Debug.Log($"GooseAttackTrigger > OnKittyHit > ATTACK_KITTY_HIT {c}");
            EventManager.TriggerEvent<AttackEvent, string, float, Collider>(AttackEvent.ATTACK_KITTY_HIT, 0f, c);
            HitTimer = 0;
            Controller.EnterFleeState();
        }
    }
}
