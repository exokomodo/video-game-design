using UnityEditor;
using UnityEngine;

/// <summary>
/// Rideable A component that can be added to a GameObject to make it rideable, making the object into a hat
/// Author: James Orson
/// </summary>
public class Rideable : MonoBehaviour
{
    #region Public fields
    public Vector3 Center = Vector3.zero;
    public Vector3 SaddleHeight = Vector3.zero;
    public float ColliderRadius = 1.0f;
    [KittyHawk.Attributes.TagSelector]
    public string RiderTag = "";
    #endregion

    #region Unity Components
    protected SphereCollider sc;
    #endregion

    #region Protected properties
    protected bool _isWithinRange { get; set; }
    protected float _max { get; set; }
    protected bool _isTriggered { get; set; }
    protected GameObject _rider = null;
    #endregion

    #region Unity hooks
    private void OnTriggerEnter(Collider c)
    {
        if (!c.CompareTag("Untagged"))
        {
            Debug.Log(c.tag);
        }
        if (c.CompareTag(RiderTag) && _rider == null)
        {
            Debug.Log("Saddle enter");
            Debug.Log("Rider entered");
            _rider = c.gameObject;
            EventManager.TriggerEvent<RiderEnterEvent, Rideable, GameObject>(
                this,
                _rider);
        }
    }

    private void Update()
    {
        if (_rider != null)
        {
            _rider.transform.position = transform.position + SaddleHeight;
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (c.CompareTag(RiderTag) && _rider != null)
        {
            Debug.Log("Saddle exit");
            EventManager.TriggerEvent<RiderExitEvent, Rideable, GameObject>(
                this,
                _rider);
            _rider = null;
        }
    }
    
    private void Start()
    {
        sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = ScaledColliderRadius();
        sc.center = transform.TransformPoint(Center);
        sc.isTrigger = true;
    }
    #endregion

    protected float ScaledColliderRadius()
    {
        Vector3 scale = gameObject.transform.lossyScale;
        float denom = Mathf.Max(scale.x, scale.y, scale.z, float.Epsilon);
        _max = 1 / denom;
        return ColliderRadius * _max;
    }

    void OnDrawGizmos()
    {
        // Display green sphere showing the collider center and radius
        Gizmos.color = Color.green;
        Vector3 center = transform.TransformPoint(Center);
        Gizmos.DrawWireSphere(center, ColliderRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(SaddleHeight, 0.1f);
    }
}
