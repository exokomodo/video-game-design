using UnityEngine;

/// <summary>
/// Script controlling player's ability to collect objects
/// Author: Calvin Ferst
/// </summary>
public class PlayerPickupController : MonoBehaviour
{

    private string catnipAudio = "CatnipMunch";

    PlayerInventory inventory;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();    
    }

    private void OnTriggerEnter(Collider other)
    {

        if (inventory == null || other.tag == null) return;

        switch(other.tag)
        {
            case "Catnip":
                inventory.Catnip = 1;
                Destroy(other.gameObject);
                EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, catnipAudio);
                break;
            case "Life":
                inventory.Lives = 1;
                Destroy(other.gameObject);
                break;
            default:
                break;
        }

    }

}
