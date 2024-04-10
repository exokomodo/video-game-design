using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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

    // Game Over content
    Canvas gameOverCanvas;
    Button restartButton;
    Button quitButton;

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

        // Set things up for Game Over

        gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        if (gameOverCanvas != null) gameOverCanvas.enabled = false;

        restartButton = GameObject.Find("GORestartButton").GetComponent<Button>();
        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
        quitButton = GameObject.Find("GOQuitButton").GetComponent<Button>();
        if (quitButton != null) quitButton.onClick.AddListener(ReturnToMenu);
    }

    private void OnDestroy()
    {
        EventManager.StopListening<PlayerDeathEvent>(OnPlayerDie);
        EventManager.StopListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);

        restartButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
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
        StartCoroutine(GameOver());
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Transition");
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        if (gameOverCanvas != null) gameOverCanvas.enabled = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        DataManager.Instance.CurrentDay = Day.MONDAY;
        DataManager.Instance.Lives = 9;
        DataManager.Instance.Catnip = 0;
        SceneManager.LoadScene("MainMenu");
    }

}
