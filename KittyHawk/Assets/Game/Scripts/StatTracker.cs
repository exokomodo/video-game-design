using UnityEngine;

public class StatTracker : MonoBehaviour
{
    public static StatTracker Instance;

    #region Event Handlers
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
    }

    private void OnDestroy()
    {
    }
    #endregion
}
