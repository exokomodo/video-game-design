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

        if (other.tag == "Catnip")
        {
            inventory.Catnip++;
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, catnipAudio);

            if (inventory.Catnip % 10 == 0 && inventory.Lives < 9) {
                inventory.Lives++;
                EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "success3");
            }
            Destroy(other.gameObject);
        }


        /* REMOVING

        switch(other.tag)
        {
            case "Catnip":
                if (inventory.Lives > 8 && inventory.Catnip > 8)
                {
                    // do nothing - max lives reached
                }
                else if (inventory.Lives < 9 & inventory.Catnip > 8)
                {
                    inventory.Lives++;
                    inventory.Catnip = 0;
                    Destroy(other.gameObject);
                    EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, catnipAudio);
                }
                else // pick up catnip
                {
                    inventory.Catnip++;
                    Destroy(other.gameObject);
                    EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, catnipAudio);
                }
                break;
            case "Life":
                inventory.Lives++;
                Destroy(other.gameObject);
                break;
            default:
                break;
        }
        */

    }

}
