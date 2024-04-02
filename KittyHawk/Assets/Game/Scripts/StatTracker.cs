using UnityEngine;

public class StatTracker : MonoBehaviour
{
    public int GeeseTrampled { get; private set; }
    public static StatTracker Instance;

    #region Event Handlers
    private void OnHorseTrampleGooseEvent()
    {
        GeeseTrampled++;
    }
    #endregion

    #region Unity Hooks
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EventManager.StartListening<HorseTrampleGooseEvent>(OnHorseTrampleGooseEvent);
    }

    private void OnDestroy()
    {
        EventManager.StopListening<HorseTrampleGooseEvent>(OnHorseTrampleGooseEvent);
    }
    #endregion
}
