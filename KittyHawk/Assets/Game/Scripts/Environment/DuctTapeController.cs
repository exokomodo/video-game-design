using UnityEngine;

/// <summary>
/// Handle tape pickup
/// Author: Calvin Ferst
/// </summary>
public class DuctTapeController : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(new Vector3(0f, 100f, 0f) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "ItemObtained");
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>("GetDuctTape", ObjectiveStatus.Completed);
            Destroy(gameObject);
        }
    }
}
