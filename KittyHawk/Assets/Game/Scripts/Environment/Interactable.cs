using UnityEditor;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [HideInInspector]
    [field: SerializeField] public Vector3 Center = Vector3.zero;
    [HideInInspector]
    [field: SerializeField] public float ColliderRadius = 2.0f;
    [HideInInspector]
    [field: SerializeField]public string InteractsWithTag = "Player";
    [HideInInspector]
    public int interactionEventIndex = 0;
    [HideInInspector]
    public string[] interactionEvent = new string[] { InteractionEvent.INTERACTION_TRIGGERED, InteractionEvent.INTERACTION_ZONE_ENTERED };
    public bool DisableOnTriggered = true;
    [HideInInspector]
    public int interactionTypeIndex = 0;
    [HideInInspector]
    public string[] interactionType = new string[] {
        InteractionType.INTERACTION_BUTTON_PRESS,
        InteractionType.INTERACTION_ITEM_PICKUP,
        InteractionType.INTERACTION_ITEM_DROP,
        InteractionType.INTERACTION_ITEM_THROW
    };
    protected SphereCollider sc;
    protected GameObject sphere;
    protected float max;
    protected bool triggered = false;
    protected Bounds bounds;

    protected void Start()
    {
        GameObject go = gameObject;
        var r = GetComponent<Renderer>();
        bounds = r.bounds;
        Vector3 scale = go.transform.localScale;
        max = 1/(Mathf.Max(scale.x, scale.y, scale.z) + float.Epsilon);
        sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = ColliderRadius * max;
        sc.center = transform.TransformPoint(Center);
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

    protected void OnTriggerExit(Collider c)
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
        string evt = interactionEvent[interactionEventIndex];
        EventManager.TriggerEvent<InteractionEvent, string, Transform, Bounds>(evt, transform, bounds);
    }

    void OnDrawGizmos()
    {
        // Display green sphere showing the collider center and radius
        Gizmos.color = Color.green;
        Vector3 center = this.transform.TransformPoint(Center);
        Gizmos.DrawWireSphere(center, ColliderRadius);
    }
}

[CustomEditor(typeof(Interactable))]
public class InteractableEditor : Editor
{
    SerializedProperty center;
    SerializedProperty colliderRadius;
    void OnEnable()
    {
        center = serializedObject.FindProperty("Center");
        colliderRadius = serializedObject.FindProperty("ColliderRadius");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Interactable script = (Interactable)target;

        script.InteractsWithTag = EditorGUILayout.TagField("Interacts with Tag", script.InteractsWithTag);
        GUILayoutOption[] options = { GUILayout.ExpandWidth(true) };
        EditorGUILayout.PropertyField(center, new GUIContent("Collider Center"), options);
        EditorGUILayout.PropertyField(colliderRadius, new GUIContent("Collider Radius"));

        GUIContent eventLabel = new GUIContent("Interaction Event");
        script.interactionEventIndex = EditorGUILayout.Popup(eventLabel, script.interactionEventIndex, script.interactionEvent);

        GUIContent typeLabel = new GUIContent("Interaction Type");
        script.interactionTypeIndex = EditorGUILayout.Popup(typeLabel, script.interactionTypeIndex, script.interactionType);

        serializedObject.ApplyModifiedProperties();
    }
}
