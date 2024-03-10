using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sets up the level (audio & fade-in)
/// TODO: keep track of objectives
/// Author: Calvin
/// </summary>
public class LevelManager : MonoBehaviour
{

    [SerializeField]
    Canvas canvas;
    [SerializeField]
    string musicName = "MainGameMusic";
    [SerializeField]
    Objective[] objectives;

    bool allObjectivesCompleted;
    Dictionary<string, Objective> objectivesDic;

    Animator anim;

    void Start()
    {
        anim = canvas.GetComponentInChildren<Animator>();
        anim.SetTrigger("FadeIn");
        EventManager.TriggerEvent<MusicEvent, string>(musicName);

        Init();
    }

    void Init()
    {
        allObjectivesCompleted = false;
        objectivesDic = new Dictionary<string, Objective>();

        foreach (Objective objective in objectives)
        {
            objectivesDic.Add(objective.ObjectiveName, objective);
        }

        EventManager.StartListening<PlayerDeathEvent>(OnPlayerDie);
        EventManager.StartListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);
    }

    private void OnDestroy()
    {
        EventManager.StopListening<PlayerDeathEvent>(OnPlayerDie);
    }

    void OnObjectiveChange(string name, ObjectiveStatus status)
    {
        if (objectivesDic[name] != null)
        {
            objectivesDic[name].Status = status;
        }

        allObjectivesCompleted = true;

        foreach (KeyValuePair<string, Objective> objective in objectivesDic)
        {
            if (objective.Value.Status != ObjectiveStatus.Completed)
            {
                allObjectivesCompleted = false;
                break;
            }
        }

        if (allObjectivesCompleted)
        {
            StartCoroutine(LevelComplete());
        }
    }

    IEnumerator LevelComplete()
    {
        // TODO: load next level
        // In this case: load cutscene where duck says congratulations
        // and says to wait for full game
        // and then loads main menu

        return null;
    }

    private void OnPlayerDie()
    {
        StartCoroutine(ReloadLevel());
    }

    public IEnumerator ReloadLevel()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
