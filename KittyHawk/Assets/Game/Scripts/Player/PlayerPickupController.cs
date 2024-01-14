using UnityEngine;

public class PlayerPickupController : MonoBehaviour
{

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
