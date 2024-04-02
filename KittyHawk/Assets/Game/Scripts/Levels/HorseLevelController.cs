using System.Linq;
using UnityEngine;

/// <summary>
/// HorseLevelController tracks the objective for the horse level
/// Author: James Orson
/// </summary>

public class HorseLevelController : MonoBehaviour
{
    private static int gooseSceneCount;
    private static HorseLevelController _instance;
    public static HorseLevelController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(HorseLevelController)) as HorseLevelController;
                if (!_instance)
                {
                    Debug.LogError($"There needs to be one active {nameof(HorseLevelController)} script on a GameObject in the horse scene.");
                }
            }
            return _instance;
        }
    }

    #region Unity lifecycle
    private void Start()
    {
        gooseSceneCount = GameObject.FindGameObjectsWithTag("Goose").Where(x => x.activeInHierarchy).Count();
        EventManager.StartListening<HorseTrampleGooseEvent>(OnHorseTrampleGooseEvent);
        EventManager.StartListening<HorseEnterPondEvent>(OnHorseEnterPondEvent);
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                "HorseObjective",
                ObjectiveStatus.InProgress);
        Debug.Log("Count of geese: " + gooseSceneCount);
    }

    private void OnHorseEnterPondEvent()
    {
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                "HorseObjective",
                ObjectiveStatus.Failed);
    }

    private void OnHorseTrampleGooseEvent()
    {
        gooseSceneCount--;
        Debug.Log("New count of geese: " + gooseSceneCount);
        if (gooseSceneCount == 0)
        {
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                "HorseObjective",
                ObjectiveStatus.Completed);
        }
    }
    #endregion
}
