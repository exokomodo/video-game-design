using System;
using UnityEngine;

public class Follower : MonoBehaviour {

    [SerializeField]
    public GameObject followTarget;

    void Start() {
        followTarget = null;
        try {
            followTarget = FindObjectsByType<FollowTarget>(FindObjectsSortMode.None)[0].gameObject;
        } catch (Exception e) {
            Debug.LogWarning($"No followTarget found. \n{e.Message}");
        }
    }

    void FixedUpdate() {
        Vector3 pos = followTarget.transform.position;
        pos.y = 1000f;
        gameObject.transform.position = pos;

        Vector3 rotEuler = followTarget.transform.eulerAngles;
        gameObject.transform.eulerAngles = new Vector3(90, rotEuler.y, -rotEuler.z);
    }
}
