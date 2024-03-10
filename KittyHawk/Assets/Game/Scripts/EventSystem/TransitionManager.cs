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
    string DialogueName;
    [SerializeField]
    string NextSceneName;
    [SerializeField]
    Image fadeImage;
    [SerializeField]
    GameObject duck;

    private Animator anim;

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
        if (name == DialogueName)
            StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(NextSceneName);
    }

    IEnumerator TriggerDialogue()
    {
        yield return new WaitForSeconds(2f);
        EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, DialogueName);
    }
}
