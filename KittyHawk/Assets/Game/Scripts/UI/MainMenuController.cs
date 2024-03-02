using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [SerializeField]
    string firstLevelName = "Alpha";
    [SerializeField]
    string highlightSoundName = "MenuHighlight";
    [SerializeField]
    string confirmSoundName = "MenuConfirm";

    public void StartGame()
    {
        PlayConfirmSound();
        //SceneManager.LoadScene(firstLevelName);
        StartCoroutine(LoadFirstScene());
    }

    public void QuitGame()
    {
        PlayConfirmSound();
        Application.Quit();
    }

    public void OpenOptionsMenu()
    {
        PlayConfirmSound();
        Debug.Log("Pretend there's an Options menu opening.");
    }

    public void PlayHighlightSound()
    {
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, highlightSoundName);
    }

    public void PlayConfirmSound()
    {
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(new Vector3(20f, 2f, -30f), confirmSoundName);
    }

    private IEnumerator LoadFirstScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(firstLevelName);
    }
}
