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
    private int maxCatnip = 0;

    void Awake()
    {
        PlayerInventory.OnCatnipChanged += UpdateCatnip;
        PlayerInventory.OnLivesChanged += UpdateLives;
    }

    void UpdateCatnip(int catnip)
    {
        catnipTextObject.text = catnip.ToString() + "/" + maxCatnip.ToString();
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
