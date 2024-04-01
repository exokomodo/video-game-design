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
    private TextMeshProUGUI bunniesTextObject;
    [SerializeField]
    private TextMeshProUGUI bunniesTotalTextObject;

    void Awake()
    {
        PlayerInventory.OnCatnipChanged += UpdateCatnip;
        PlayerInventory.OnLivesChanged += UpdateLives;
        PlayerInventory.OnBunniesChanged += UpdateBunnies;
        PlayerInventory.OnBunniesTotalChanged += UpdateBunniesTotal;
    }

    void UpdateCatnip(int catnip)
    {
        catnipTextObject.text = catnip.ToString();
    }

    void UpdateLives(int lives)
    {
        lifeTextObject.text = lives.ToString();
    }

    void UpdateBunnies(int bunnies)
    {
        bunniesTextObject.text = bunnies.ToString();
    }

    void UpdateBunniesTotal(int bunniesTotal)
    {
        bunniesTotalTextObject.text = bunniesTotal.ToString();
    }

    void OnDestroy()
    {
        PlayerInventory.OnCatnipChanged -= UpdateCatnip;
        PlayerInventory.OnLivesChanged -= UpdateLives;
        PlayerInventory.OnBunniesChanged -= UpdateBunnies;
        PlayerInventory.OnBunniesTotalChanged -= UpdateBunniesTotal;
    }

}
