using UnityEngine;

public class PlayerInventoryUIController : MonoBehaviour
{

    void Start()
    {
        PlayerInventory.OnCatnipChanged += UpdateCatnip;
        PlayerInventory.OnLivesChanged += UpdateLives;
    }

    void UpdateCatnip(int catnip)
    {
        Debug.Log($"Kitty Hawk has {catnip} catnip.");
    }

    void UpdateLives(int lives)
    {
        Debug.Log("Kitty Hawk has " + lives + " lives.");
    }

    void OnDestroy()
    {
        PlayerInventory.OnCatnipChanged -= UpdateCatnip;
        PlayerInventory.OnLivesChanged -= UpdateLives;
    }

}
