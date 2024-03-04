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
    [SerializeField]
    string menuMusicName = "MenuMusic";
    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    GameObject creditsPanel;
    [SerializeField]
    GameObject optionsPanel;

    private void Start()
    {
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);

        // TODO: Add music event to Audio Manager and use that instead
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(mainCamera.transform.position, menuMusicName);
    }

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

    public void ToggleOptionsMenu()
    {
        PlayConfirmSound();
        
        if (optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
        }
        else
        {
            optionsPanel.SetActive(true);
        }
    }

    public void ToggleCreditsMenu()
    {
        PlayConfirmSound();
        
        if (creditsPanel.activeSelf)
        {
            creditsPanel.SetActive(false);
        }
        else
        {
            creditsPanel.SetActive(true);
        }
    }

    public void SetSoundVolume(float value)
    {
        Debug.Log("Sound volume is now " + value);
        AudioManager.instance.SoundVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        Debug.Log("Music volume is now " + value);
        AudioManager.instance.MusicVolume = value;
    }

    public void PlayHighlightSound()
    {
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(mainCamera.transform.position, highlightSoundName);
    }

    public void PlayConfirmSound()
    {
        // EventManager.TriggerEvent<AudioEvent, Vector3, string>(new Vector3(20f, 2f, -30f), confirmSoundName);
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(mainCamera.transform.position, confirmSoundName);
    }

    private IEnumerator LoadFirstScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(firstLevelName);
    }
}
