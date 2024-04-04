using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    [SerializeField]
    string nextLevel;
    [SerializeField]
    bool displayDay = true;

    bool allObjectivesCompleted;
    Dictionary<string, Objective> objectivesDic;

    Animator anim;

    void Start()
    {
        DisplayDayName();

        anim = canvas.GetComponentInChildren<Animator>();
        anim.SetTrigger("FadeIn");
        EventManager.TriggerEvent<MusicEvent, string>(musicName);

        Init();
    }

    void DisplayDayName()
    {

        TextMeshProUGUI dayText = null;
        GameObject dayTextObject = GameObject.Find("DayName");

        if (dayTextObject != null)
        {
            dayText = dayTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            return;
        }

        if (displayDay)
        {
            dayText.text = DataManager.Instance.CurrentDay.ToString();
        }
        else
        {
            dayText.text = "";
        }
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
        EventManager.StopListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);
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
            LevelComplete();
        }
    }

    void LevelComplete()
    {
        Debug.Log("YAY YOU BEAT THE LEVEL");
        StartCoroutine(LoadNextLevel());
    }

    private void OnPlayerDie()
    {
        DataManager.Instance.Lives = 9;
        DataManager.Instance.Catnip = 0;
        StartCoroutine(ReloadLevel());
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Transition");
    }

    public IEnumerator ReloadLevel()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // TODO: TESTING! COMMENT OUT!
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            LevelComplete();
    } */

}
