using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// HorseLevelController tracks the objective for the horse level
/// Author: James Orson
/// </summary>

public class HorseLevelController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gooseTextObject;
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

    #region Protected methods
    protected void CountGeese()
    {
        gooseSceneCount = GameObject.FindGameObjectsWithTag("Goose")
            .Count(x => x.GetComponent<GooseAI>().IsAlive);
        gooseTextObject.text = gooseSceneCount.ToString();
    }
    #endregion

    #region Unity lifecycle
    private void Start()
    {
        CountGeese();
        EventManager.StartListening<HorseTrampleGooseEvent>(OnHorseTrampleGooseEvent);
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

    private void OnHorseTrampleGooseEvent()
    {
        CountGeese();
        if (gooseSceneCount <= 0)
        {
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                "HorseObjective",
                ObjectiveStatus.Completed);
        }
    }
    #endregion
}
