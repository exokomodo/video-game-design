using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Sets up the level (audio & fade-in), handle objectives, game over
/// Authors: Calvin & Geoff
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
    [SerializeField]
    public GameObject MarkerPrefab;
    [HideInInspector]
    public Dictionary<string, Objective> objectivesDic { get; private set; }

    bool allObjectivesCompleted;


    Animator anim;

    // Game Over content
    Canvas gameOverCanvas;
    Button restartButton;
    Button quitButton;


    void Awake()
    {
        // DisplayDayName();

        anim = canvas.GetComponentInChildren<Animator>();
        anim.SetTrigger("FadeIn");
        // EventManager.TriggerEvent<MusicEvent, string>(musicName); ISSUE

        Init();
        Invoke("Recenter", 0.5f);
    }

    private void Start()
    {
        EventManager.TriggerEvent<MusicEvent, string>(musicName);
        DisplayDayName();
    }

    void Recenter() {
        try {
            CameraController camController = Camera.main.GetComponent<CameraController>();
            camController.LevelStartRecenter();
        } catch (Exception e) {
            Debug.LogWarning(e);
        }
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
            AddObjective(objective);
        }

        EventManager.StartListening<PlayerDeathEvent>(OnPlayerDie);
        EventManager.StartListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);

        // Set things up for Game Over

        try
        {
            gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
            if (gameOverCanvas != null) gameOverCanvas.enabled = false;

            restartButton = GameObject.Find("GORestartButton").GetComponent<Button>();
            if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
            quitButton = GameObject.Find("GOQuitButton").GetComponent<Button>();
            if (quitButton != null) quitButton.onClick.AddListener(ReturnToMenu);
        }
        catch (Exception e)
        {
            Debug.Log("Couldn't find game over panel.");
        }
        
    }

    public void AddObjective(Objective obj)
    {
        try {
            obj.Status = ObjectiveStatus.NotStarted;
            objectivesDic.Add(obj.ObjectiveName, obj);
        } catch (Exception e) {
            Debug.Log($"Could NOT add objective: {obj}, {objectivesDic}");
            Debug.LogWarning(e);
        }

    }

    private void OnDestroy()
    {
        EventManager.StopListening<PlayerDeathEvent>(OnPlayerDie);
        EventManager.StopListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);

        try
        {
            restartButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
        }
        catch (Exception e)
        {
            Debug.Log("No Game over canvas found.");
        }
        
    }

    void OnObjectiveChange(string name, ObjectiveStatus status)
    {
        Debug.Log($"OnObjectiveChange: {name}");
        if (objectivesDic[name] != null)
        {
            Debug.Log($"OnObjectiveChange: {name} FOUND");
            objectivesDic[name].Status = status;
        }

        allObjectivesCompleted = true;

        foreach (KeyValuePair<string, Objective> objective in objectivesDic)
        {
            Objective obj = objective.Value;
            if (obj.Required && obj.Status != ObjectiveStatus.Completed)
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
        Debug.Log("LevelManager > YAY YOU BEAT THE LEVEL");
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
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(restartButton.gameObject);
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

    public Objective CreateObjective(string name, Vector3 location, float scale=0.2f, Transform target=null, bool required=false) {
        Objective obj = ScriptableObject.CreateInstance<Objective>();
        obj.ObjectiveName = name;
        obj.Required = required;
        obj.FollowTarget = target;
        obj.ShowMarker = true;
        obj.MarkerPrefab = MarkerPrefab;
        obj.MarkerLocation = location;
        obj.Scale = new Vector3(scale, scale, scale);
        return obj;
    }

}
