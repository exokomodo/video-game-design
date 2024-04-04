using UnityEngine;

/// <summary>
/// Handle key pickup
/// Author: Calvin Ferst
/// </summary>
public class BarnKeyController : MonoBehaviour
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
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>("GetBarnKey", ObjectiveStatus.Completed);
            Destroy(gameObject);
        }
    }

}
