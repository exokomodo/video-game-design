/// <summary>
/// StatTracker provides a centralized location for event listeners to be tracked for statistics displays and player progress
/// Author: James Orson
/// </summary>

using UnityEngine;

public class StatTracker : MonoBehaviour
{
    public static StatTracker Instance;

    public int HorseSwimSessions { get; private set; } = 0;
    public int GeeseTrampled { get; private set; } = 0;

    #region Event Handlers
    private void OnHorseEnterPondEvent()
    {
        ++HorseSwimSessions;
    }

    private void OnHorseTrampleGooseEvent()
    {
        ++GeeseTrampled;
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
        EventManager.StartListening<HorseEnterPondEvent>(OnHorseEnterPondEvent);
        EventManager.StartListening<HorseTrampleGooseEvent>(OnHorseTrampleGooseEvent);
    }

    private void OnDestroy()
    {
        EventManager.StopListening<HorseEnterPondEvent>(OnHorseEnterPondEvent);
        EventManager.StopListening<HorseTrampleGooseEvent>(OnHorseTrampleGooseEvent);
    }
    #endregion
}
