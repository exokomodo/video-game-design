using UnityEngine;

public class VelocityReporter : MonoBehaviour {
    private Vector3 prevPos;
    public Vector3 rawVelocity { get; private set; }
    public Vector3 velocity { get; private set; }
    public float smoothingTimeFactor = 0.5f;
    private Vector3 smoothingParamVel;

    void Start () {
        prevPos = this.transform.position;
    }
    void Update () {
        if (!Mathf.Approximately(Time.deltaTime, 0f)) {
            rawVelocity = (this.transform.position - prevPos) / Time.deltaTime;
            velocity = Vector3.SmoothDamp(velocity, rawVelocity, ref smoothingParamVel, smoothingTimeFactor);
        }
        else
        {
            rawVelocity = Vector3.zero;
            velocity = Vector3.zero;
        }
        prevPos = this.transform.position;
    }
}
