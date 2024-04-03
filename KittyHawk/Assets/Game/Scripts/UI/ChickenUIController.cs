using UnityEngine;
using TMPro;

/// <summary>
/// Class in charge of updating player inventory UI
/// Author: Paul Garza
/// </summary>
public class ChickenUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI chickenTextObject;

    void Awake()
    {
        PlayerInventory.OnChickensChanged += UpdateChickens;
    }

    void UpdateChickens(int chickens)
    {
        chickenTextObject.text = chickens.ToString();
    }

    void OnDestroy()
    {
        PlayerInventory.OnChickensChanged -= UpdateChickens;
    }

}
