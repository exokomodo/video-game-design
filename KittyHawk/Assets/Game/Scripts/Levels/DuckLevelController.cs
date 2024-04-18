using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DuckLevelController manages Objective Markers
/// Author: Geoffrey Roth
/// </summary>
public class DuckLevelController : MonoBehaviour {

    [SerializeField]
    private LevelManager levelManager;

    public static class Objectives {
        public static string COW = "Level2_Cow";
        public static string KEY = "GetBarnKey";
        public static string DUCT_TAPE = "GetDuctTape";
        public static string BARN = "Level2_Barn";
        public static string HAMMER = "GetHammer";
        public static string TRACTOR = "FixTractor";
    }

    private bool hasHammer = false;
    private bool hasTape = false;

    // objectiveMap sequences objectives
    // completing the objective denoted by the key displays markers for the
    // corresponding list of new objectives
    private Dictionary<string, List<string>> objectiveMap = new Dictionary<string, List<string>>{
        {Objectives.COW,        new List<string>{Objectives.KEY, Objectives.DUCT_TAPE}},
        {Objectives.KEY,        new List<string>{Objectives.BARN}},
        {Objectives.BARN,       new List<string>{Objectives.HAMMER}},
    };

    void Start() {
        EventManager.StartListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);
        EventManager.StartListening<DialogueOpenEvent, Vector3, string>(OnDialogOpen);
        Invoke("Init", 0.1f); // Delay required to avoid race condition with LevelManager
    }

    private void Init() {
        // Initial objective designated here
        UpdateObjective(Objectives.COW, ObjectiveStatus.InProgress);
    }

    private void OnObjectiveChange(string name, ObjectiveStatus status) {
        if (status == ObjectiveStatus.Completed) {
            if (objectiveMap.ContainsKey(name)) {
                foreach (string objName in objectiveMap[name]) {
                    Objective pending = levelManager.objectivesDic[objName];
                    if (pending.Status != ObjectiveStatus.Completed)
                        UpdateObjective(objName, ObjectiveStatus.InProgress);
                }
            } else if (name == Objectives.HAMMER || name == Objectives.DUCT_TAPE) {
                if (name == Objectives.HAMMER) hasHammer = true;
                if (name == Objectives.DUCT_TAPE) hasTape = true;
                if (hasHammer && hasTape) {
                    UpdateObjective(Objectives.TRACTOR, ObjectiveStatus.InProgress);
                }

            }
        }
    }

    private void OnDialogOpen(Vector3 position, string dialogueName) {
        switch (dialogueName) {
            case "CowDuckDialogue":
                UpdateObjective(Objectives.COW, ObjectiveStatus.Completed);
                break;
        }
    }

    private void UpdateObjective(string name, ObjectiveStatus status) {
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(name, status);
    }

    private void OnDestroy() {
        EventManager.StopListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);
        EventManager.StopListening<DialogueOpenEvent, Vector3, string>(OnDialogOpen);
    }
}
