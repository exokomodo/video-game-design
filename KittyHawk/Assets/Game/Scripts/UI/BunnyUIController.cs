using UnityEngine;
using TMPro;

/// <summary>
/// Class in charge of updating bunny UI
/// Author: Geoffrey Roth
/// </summary>
public class BunnyUIController : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI bunniesTextObject;
    [SerializeField]
    private TextMeshProUGUI bunniesTotalTextObject;

    void Awake() {
        PlayerInventory.OnBunniesChanged += UpdateBunnies;
        PlayerInventory.OnBunniesTotalChanged += UpdateBunniesTotal;
    }

    void UpdateBunnies(int bunnies) {
        bunniesTextObject.text = bunnies.ToString();
    }

    void UpdateBunniesTotal(int bunniesTotal){
        bunniesTotalTextObject.text = bunniesTotal.ToString();
    }

    void OnDestroy() {
        PlayerInventory.OnBunniesChanged -= UpdateBunnies;
        PlayerInventory.OnBunniesTotalChanged -= UpdateBunniesTotal;
    }
}
