using UnityEngine;

/// <summary>
/// HorseObjectiveController tracks the objective for the horse level
/// Author: James Orson
/// </summary>

public class HorseObjectiveController : MonoBehaviour
{
    private static int gooseSceneCount;
    private static HorseObjectiveController _instance;
    public static HorseObjectiveController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(HorseObjectiveController)) as HorseObjectiveController;
                if (!_instance)
                {
                    Debug.LogError($"There needs to be one active {nameof(HorseObjectiveController)} script on a GameObject in the horse scene.");
                }
            }
            return _instance;
        }
    }

    #region Unity lifecycle
    private void Start()
    {
        EventManager.StartListening<HorseTrampleGooseEvent, GameObject>(OnHorseTrampleGooseEvent);
        gooseSceneCount = GameObject.FindGameObjectsWithTag("Goose").Length;
        EventManager.StartListening<HorseEnterPondEvent>(OnHorseEnterPondEvent);
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                "HorseObjective",
                ObjectiveStatus.InProgress);
    }

    private void OnHorseEnterPondEvent()
    {
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                "HorseObjective",
                ObjectiveStatus.Failed);
    }

    private void OnHorseTrampleGooseEvent(GameObject goose)
    {
        gooseSceneCount--;
        if (gooseSceneCount == 0)
        {
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                "HorseObjective",
                ObjectiveStatus.Completed);
        }
    }
    #endregion
}
