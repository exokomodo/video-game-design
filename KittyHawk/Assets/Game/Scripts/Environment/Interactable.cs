using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float ColliderRadius = 2.0f;
    public float ColliderOffsetX = 0.0f;
    public float ColliderOffsetY = 0.0f;
    public float ColliderOffsetZ = 0.0f;
    public string InteractsWithTag = "Player";
    public string InteractionEventName = "InteractionEnabled";
    public bool DisableOnTriggered = true;
    protected SphereCollider sc;
    protected GameObject sphere;
    protected float max;
    protected bool triggered = false;

    protected void Start()
    {
        GameObject go = gameObject;
        Vector3 scale = go.transform.localScale;
        max = 1/(Mathf.Max(scale.x, scale.y, scale.z) + float.Epsilon);
        sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = ColliderRadius * max;
        sc.center = transform.TransformPoint(new Vector3(ColliderOffsetX, ColliderOffsetY, ColliderOffsetZ));
        sc.isTrigger = true;
    }
    protected void OnTriggerEnter(Collider c)
    {
        if (((DisableOnTriggered && !triggered) || (!DisableOnTriggered)) && c.transform.root.CompareTag(InteractsWithTag))
        {
            triggered = true;
            TriggerInteraction(c);
        }
    }

    public void TriggerInteraction(Collider c)
    {
        // Trigger an event to let listeners know an interaction event is now possible
        EventManager.TriggerEvent<InteractionEvent, string, Transform>(InteractionEventName, transform);
    }

    void OnDrawGizmos()
    {
        // Display green sphere showing the collider center and radius
        Gizmos.color = Color.green;
        Vector3 center = this.transform.TransformPoint(new Vector3(ColliderOffsetX, ColliderOffsetY, ColliderOffsetZ));
        Gizmos.DrawWireSphere(center, ColliderRadius);
    }
}
