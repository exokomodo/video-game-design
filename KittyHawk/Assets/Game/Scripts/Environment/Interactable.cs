using UnityEditor;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [HideInInspector]
    [field: SerializeField] public Vector3 Center = Vector3.zero;
    [HideInInspector]
    [field: SerializeField] public float ColliderRadius = 1.0f;
    [HideInInspector]
    [field: SerializeField]public string InteractsWithTag = "Player";
    [HideInInspector]
    public int interactionEventIndex = 0;
    [HideInInspector]
    public string[] interactionEvent = new string[] {
        InteractionEvent.INTERACTION_TRIGGERED,
        InteractionEvent.INTERACTION_ZONE_ENTERED,
        InteractionEvent.INTERACTION_ZONE_EXITED
    };
    public bool DisableOnTriggered = false;
    [HideInInspector]
    public int interactionTypeIndex = 0;
    [HideInInspector]
    public string[] interactionType = new string[] {
        InteractionType.NONE,
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
        string evt = interactionEvent[interactionEventIndex];
        evt = evt == InteractionEvent.INTERACTION_ZONE_EXITED? InteractionEvent.INTERACTION_ZONE_ENTERED : evt;
        if (c.transform.root.CompareTag(InteractsWithTag) &&
            ((DisableOnTriggered && !triggered) || (!DisableOnTriggered)))
        {
            triggered = true;
            TriggerEvent(evt);
        }
    }

    protected void OnTriggerExit(Collider c)
    {
        if (c.transform.root.CompareTag(InteractsWithTag) &&
            ((DisableOnTriggered && !triggered) || (!DisableOnTriggered)))
        {
            triggered = true;
            TriggerEvent(InteractionEvent.INTERACTION_ZONE_EXITED);
        }
    }

    protected void TriggerEvent(string evt)
    {
        string typ = interactionType[interactionTypeIndex];
        EventManager.TriggerEvent<InteractionEvent, string, string, Transform, Bounds>(evt, typ, transform, bounds);
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
    SerializedProperty disableOnTriggered;
    void OnEnable()
    {
        center = serializedObject.FindProperty("Center");
        colliderRadius = serializedObject.FindProperty("ColliderRadius");
        disableOnTriggered = serializedObject.FindProperty("DisableOnTriggered");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Interactable script = (Interactable)target;

        script.InteractsWithTag = EditorGUILayout.TagField("Interacts with Tag", script.InteractsWithTag);
        GUILayoutOption[] options = { GUILayout.ExpandWidth(true) };
        EditorGUILayout.PropertyField(center, new GUIContent("Collider Center"), options);
        EditorGUILayout.PropertyField(colliderRadius, new GUIContent("Collider Radius"));
        EditorGUILayout.PropertyField(disableOnTriggered, new GUIContent("Disable Once Triggered"));

        GUIContent eventLabel = new GUIContent("Interaction Event");
        script.interactionEventIndex = EditorGUILayout.Popup(eventLabel, script.interactionEventIndex, script.interactionEvent);

        GUIContent typeLabel = new GUIContent("Interaction Type");
        script.interactionTypeIndex = EditorGUILayout.Popup(typeLabel, script.interactionTypeIndex, script.interactionType);

        serializedObject.ApplyModifiedProperties();
    }
}
