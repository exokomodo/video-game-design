using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockingChairController : MonoBehaviour
{

    [SerializeField]
    Transform centerOfGravity;
    [SerializeField]
    float collisionThreshold = 0.25f;
    [SerializeField]
    string audioName = "Sound/CreakingChair";

    private Rigidbody rb;
    private float nextTriggerTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfGravity.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > collisionThreshold &&
            Time.timeSinceLevelLoad > nextTriggerTime)
        {
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, audioName);
            nextTriggerTime = Time.timeSinceLevelLoad + 1f;
        }
    }


}
