using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// HorseLevelController tracks the objective for the horse level
/// Author: James Orson
/// </summary>

public class HorseLevelController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gooseTextObject;
    [SerializeField]
    private LevelManager levelManager;
    private const string MOUNT_OBJECTIVE_NAME = "MountHorseObjective";
    private const string LEVEL_OBJECTIVE_NAME = "HorseObjective";
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
        UpdateUI();
    }

    protected void UpdateUI()
    {
        gooseTextObject.text = gooseSceneCount.ToString();
    }
    #endregion

    #region Unity lifecycle
    private void Start()
    {
        CountGeese();
        EventManager.StartListening<HorseTrampleGooseEvent>(OnHorseTrampleGooseEvent);
        EventManager.StartListening<HorseEnterPondEvent>(OnHorseEnterPondEvent);
        EventManager.StartListening<RiderEnterEvent>(OnMount);
        SetObjectives();
    }

    private void SetObjectives() {
        Vector3 pos = transform.position;
        Objective mountObjective = levelManager.CreateObjective(
            MOUNT_OBJECTIVE_NAME,
            new Vector3(pos.x, 3f, pos.z),
            0.1f,
            transform
        );
        levelManager.AddObjective(mountObjective);
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                MOUNT_OBJECTIVE_NAME,
                ObjectiveStatus.InProgress);
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                LEVEL_OBJECTIVE_NAME,
                ObjectiveStatus.InProgress);
    }

    private void OnMount() {
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                MOUNT_OBJECTIVE_NAME,
                ObjectiveStatus.Completed);
    }

    private void OnHorseEnterPondEvent()
    {
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                LEVEL_OBJECTIVE_NAME,
                ObjectiveStatus.Failed);
    }

    private void OnHorseTrampleGooseEvent()
    {
        gooseSceneCount--;
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "GooseHit1");
        UpdateUI();
        if (gooseSceneCount <= 0)
        {
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(
                LEVEL_OBJECTIVE_NAME,
                ObjectiveStatus.Completed);
        }
    }
    #endregion
}
