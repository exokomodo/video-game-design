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
    [SerializeField]
    private TextMeshProUGUI chickenTextObject;

    void Awake()
    {
        PlayerInventory.OnCatnipChanged += UpdateCatnip;
        PlayerInventory.OnLivesChanged += UpdateLives;
        PlayerInventory.OnChickensChanged += UpdateChickens;
    }

    void UpdateCatnip(int catnip)
    {
        catnipTextObject.text = catnip.ToString();
    }

    void UpdateLives(int lives)
    {
        lifeTextObject.text = lives.ToString();
    }

    void UpdateChickens(int chickens)
    {
        chickenTextObject.text = chickens.ToString();
    }

    void OnDestroy()
    {
        PlayerInventory.OnCatnipChanged -= UpdateCatnip;
        PlayerInventory.OnLivesChanged -= UpdateLives;
        PlayerInventory.OnChickensChanged -= UpdateChickens;
    }

}
