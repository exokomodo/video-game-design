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
        if (name == dialogueName)
            StartCoroutine(LoadNextScene());
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
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }

    IEnumerator TriggerDialogue()
    {
        yield return new WaitForSeconds(2f);

        switch (DataManager.Instance.CurrentDay)
        {
            case Day.MONDAY:
                // TO-DO - Replace dialogue name with appropriate end-of-day dialogue
                dialogueName = "ChickenLevelDone";
                break;
            case Day.TUESDAY:
                dialogueName = "DuckLevelDone";
                break;
            case Day.WEDNESDAY:
                // TO-DO - Replace dialogue name with appropriate end-of-day dialogue
                dialogueName = "CowObjectiveComplete";
                break;
            case Day.THURSDAY:
                dialogueName = "BunnyCompleteDialogueDuck";
                break;
            case Day.FRIDAY:
                dialogueName = "HorseComplete";
                break;
        }
        // NOTE: Moves to next day
        DataManager.Instance.CurrentDay += 1;
        EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, dialogueName);
    }
}
