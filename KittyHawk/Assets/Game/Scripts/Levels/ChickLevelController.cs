using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private Dictionary<string, List<string>> objectiveMap = new Dictionary<string,List<string>>{
        {Objectives.COW, new List<string>{Objectives.KEY, Objectives.CHICKEN}},
        {Objectives.KEY, new List<string>{Objectives.CHICKEN, Objectives.COOP}},
        {Objectives.CHICKEN, new List<string>{Objectives.KEY, Objectives.COOP}}
    };
    void Start() {
        // Debug.Log("Objective > START");
        EventManager.StartListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);
        EventManager.StartListening<DialogueOpenEvent, Vector3, string>(OnDialogOpen);
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogClose);
        Invoke("Init", 0.1f);
    }

    private void Init() {
        UpdateObjective(Objectives.COW, ObjectiveStatus.InProgress);
    }

    private void OnObjectiveChange(string name, ObjectiveStatus status) {
        if (status == ObjectiveStatus.Completed) {
            if (objectiveMap.ContainsKey(name)) {
                foreach (string objName in objectiveMap[name]) {
                    Debug.Log($"objective > objName > {objName}");
                    Objective pending = levelManager.objectivesDic[objName];
                    Debug.Log($"objective > pending name > {pending.ObjectiveName}, status: {pending.Status}");
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

    private void OnDialogClose(string dialogueName)
    {

    }

    private void UpdateObjective(string name, ObjectiveStatus status) {
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(name, status);
    }

    private void OnDestroy() {
        EventManager.StopListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);
    }
}
