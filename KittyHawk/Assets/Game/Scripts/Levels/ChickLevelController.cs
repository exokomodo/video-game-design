using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ChickLevelController manages Objective Markers
/// Author: Geoffrey Roth
/// </summary>
public class ChickLevelController : MonoBehaviour {

    [SerializeField]
    private LevelManager levelManager;

    private class Objectives {
        public static string COW = "Level1_Cow";
        public static string KEY = "Objective_Key";
        public static string CHICKEN = "Level1_Chicken";
        public static string COOP = "Level1_Coop";
        public static string CHICKS = "ChickObjective";
    }

    // objectiveMap sequences objectives
    // completing the objective denoted by the key displays markers for the
    // corresponding list of new objectives
    private Dictionary<string, List<string>> objectiveMap = new Dictionary<string, List<string>>{
        {Objectives.COW,        new List<string>{Objectives.KEY, Objectives.CHICKEN}},
        {Objectives.KEY,        new List<string>{Objectives.CHICKEN, Objectives.COOP}},
        {Objectives.CHICKEN,    new List<string>{Objectives.KEY, Objectives.COOP}}
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
                    // Debug.Log($"objective > objName > {objName}");
                    Objective pending = levelManager.objectivesDic[objName];
                    // Debug.Log($"objective > pending name > {pending.ObjectiveName}, status: {pending.Status}");
                    if (pending.Status != ObjectiveStatus.Completed)
                        UpdateObjective(objName, ObjectiveStatus.InProgress);
                }
            }
        }
    }

    private void OnDialogOpen(Vector3 position, string dialogueName) {
        switch (dialogueName) {
            case "CowDialoguePaul":
                UpdateObjective(Objectives.COW, ObjectiveStatus.Completed);
                break;
            case "ChickenDialogue":
                UpdateObjective(Objectives.CHICKEN, ObjectiveStatus.Completed);
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
