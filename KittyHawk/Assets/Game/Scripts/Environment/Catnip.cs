using UnityEngine;

public class Catnip : MonoBehaviour
{

    [SerializeField]
    int catnipAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.Catnip = catnipAmount;
            Destroy(gameObject);
        }
    }

}
