using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    Image fadeImage;
    private Animator anim;

    void Start()
    {
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogueFinished);
        anim = fadeImage.GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogueFinished);
    }

    public void TriggerInitialDialogue()
    {
        EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "DuckIntroduction");
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    private void OnDialogueFinished(string dialogueName)
    {
        Debug.Log("DIALOGUE FINISHED BEING CALLED");
        Debug.Log("DIALOGUE NAME IS: " + dialogueName);
        if (dialogueName == "DuckIntroduction")
        {
            StartCoroutine(LoadGame());
        }
    }

    IEnumerator LoadGame()
    {
        Debug.Log("LOADING ALPHA");
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("PaulScene");
    }

    public void SkipTutorial()
    {
        StartCoroutine(LoadGame());
    }
}
