using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class for handling transitions between scenes
/// Author: Calvin Ferst
/// </summary>
public class TransitionManager : MonoBehaviour
{

    [SerializeField]
    Image fadeImage;
    [SerializeField]
    GameObject duck;

    private Animator anim;
    private string dialogueName;

    void Start()
    {
        anim = fadeImage.GetComponent<Animator>();
        duck.GetComponentInChildren<SmokingDuckController>().ToggleTalking(true);

        StartCoroutine(TriggerDialogue());

        EventManager.StartListening<DialogueCloseEvent, string>(TriggerLoadNextScene);
    }

    private void OnDestroy()
    {
        EventManager.StopListening<DialogueCloseEvent, string>(TriggerLoadNextScene);
    }

    public void TriggerLoadNextScene(string name)
    {
        Debug.Log("!DIALOGUE NAME IS: " + name);

        if (name == dialogueName)
        {
            Debug.Log("!LOADING NEXT SCENE! TODAY IS: " + DataManager.Instance.CurrentDay.ToString());
            DataManager.Instance.Catnip = 0;
            StartCoroutine(LoadNextScene());
        }

    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);

        switch (DataManager.Instance.CurrentDay)
        {
            case (Day.MONDAY):
                DataManager.Instance.CurrentDay = Day.TUESDAY;
                SceneManager.LoadScene("DuckIntro");
                break;
            case (Day.TUESDAY):
                DataManager.Instance.CurrentDay = Day.WEDNESDAY;
                SceneManager.LoadScene("BenScene");
                break;
            case (Day.WEDNESDAY):
                DataManager.Instance.CurrentDay = Day.THURSDAY;
                SceneManager.LoadScene("LevelBunnyHop");
                break;
            case (Day.THURSDAY):
                DataManager.Instance.CurrentDay = Day.FRIDAY;
                SceneManager.LoadScene("HorseLevel");
                break;
            case (Day.FRIDAY):
                DataManager.Instance.CurrentDay = Day.MONDAY;
                DataManager.Instance.Lives = 9;
                DataManager.Instance.Catnip = 0;
                SceneManager.LoadScene("TheEnd");
                break;
        }
    }

    IEnumerator TriggerDialogue()
    {
        yield return new WaitForSeconds(2f);

        switch (DataManager.Instance.CurrentDay)
        {
            case Day.MONDAY:
                dialogueName = "ChickenLevelDone";
                break;
            case Day.TUESDAY:
                dialogueName = "DuckLevelDone";
                break;
            case Day.WEDNESDAY:
                dialogueName = "CowObjectiveComplete";
                break;
            case Day.THURSDAY:
                dialogueName = $"BunnyCompleteDialogue{Random.Range(1, 6)}";
                break;
            case Day.FRIDAY:
                dialogueName = "HorseComplete";
                break;
        }

        EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, dialogueName);
    }
}
