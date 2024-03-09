using UnityEngine;
using TMPro;

/// <summary>
/// Class in charge of updating player inventory UI
/// Author: Calvin Ferst
/// </summary>
public class PlayerInventoryUIController : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI catnipTextObject;
    [SerializeField]
    private TextMeshProUGUI lifeTextObject;

    void Start()
    {
        PlayerInventory.OnCatnipChanged += UpdateCatnip;
        PlayerInventory.OnLivesChanged += UpdateLives;
    }

    void UpdateCatnip(int catnip)
    {
        catnipTextObject.text = catnip.ToString();
    }

    void UpdateLives(int lives)
    {
        lifeTextObject.text = lives.ToString();
    }

    void OnDestroy()
    {
        PlayerInventory.OnCatnipChanged -= UpdateCatnip;
        PlayerInventory.OnLivesChanged -= UpdateLives;
    }

}
