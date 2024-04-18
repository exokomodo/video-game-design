using UnityEngine;

/// <summary>
/// Simple script to bob a GameObject up and down and rotate it
/// Author: Geoffrey Roth
/// Adapted from: https://forum.unity.com/threads/how-to-make-an-object-move-up-and-down-on-a-loop.380159/
/// </summary>
public class ArrowMarker : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;

    [SerializeField]
    float height = 0.5f;

    public Transform FollowTarget;

    private Vector3 origin;
    private Vector3 offset = Vector3.zero;

    void Start() {
        origin = transform.position;
    }

    void Update() {
        if (FollowTarget) {
            offset = FollowTarget.position - origin;
            offset.y = 0;
        }

    }

    void FixedUpdate() {
        float newY = Mathf.Sin(Time.fixedTime * speed);
        transform.position = origin + offset + new Vector3(0, newY, 0) * height;
        transform.Rotate(0, 0, 1);
    }
}
