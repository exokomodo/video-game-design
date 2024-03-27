using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the party scne introducing trash level
/// Author: Calvin Ferst
/// </summary>
public class PartyManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] geese;
    [SerializeField]
    float delay = 4f;
    [SerializeField]
    Image fadeImage;

    private Animator anim;

    private void Start()
    {
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogueFinished);
        anim = fadeImage.GetComponent<Animator>();

        StartCoroutine(Party());
    }

    private void OnDestroy()
    {
        EventManager.StopListening<DialogueCloseEvent, string>(OnDialogueFinished);
    }

    IEnumerator Party()
    {
        yield return new WaitForSeconds(4f);

        foreach (GameObject goose in geese)
        {
            Animator anim = goose.GetComponentInChildren<Animator>();

            if (anim != null)
                anim.SetBool("PartyTime", true);
        }
    }

    public void TriggerInitialDialogue()
    {
        EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "GoosePartyIntro");
    }

    private void OnDialogueFinished(string dialogueName)
    {
        if (dialogueName == "GoosePartyIntro")
        {
            StartCoroutine(LoadGame());
        }
    }

    IEnumerator LoadGame()
    {
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        Debug.Log("Loading next level");
        // SceneManager.LoadScene("DuckLevel");
    }

    public void SkipCutscene()
    {
        StartCoroutine(LoadGame());
    }

}
