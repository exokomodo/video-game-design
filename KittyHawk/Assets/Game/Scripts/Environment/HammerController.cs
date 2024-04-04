using UnityEngine;

/// <summary>
/// Handle hammer pickup
/// Author: Calvin Ferst
/// </summary>
public class HammerController : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "ItemObtained");
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>("GetHammer", ObjectiveStatus.Completed);
            Destroy(gameObject);
        }
    }

}
