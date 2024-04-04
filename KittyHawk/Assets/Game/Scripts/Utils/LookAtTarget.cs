using UnityEngine;

public class LookAtTarget : MonoBehaviour {

    [SerializeField]
    public GameObject target;

    void FixedUpdate() {
        Vector3 direction = Vector3.Normalize(target.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.fixedDeltaTime * 2f
        );
    }
}


