using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Class controlling pause menu and game pause/time
/// Author: Calvin Ferst
/// </summary>
public class PauseMenuController : MonoBehaviour
{

    [SerializeField]
    string confirmSoundName = "MenuConfirm";
    [SerializeField]
    Canvas pauseCanvas;

    bool isPaused;

    private Button resumeButton;
    private Button restartButton;
    private Button quitButton;
    private Slider soundSlider;
    private Slider musicSlider;

    private InputReader input;

    void Start()
    {
        pauseCanvas.enabled = false;

        isPaused = false;

        SetInitialVolumes();
        FindButtons();

        input = GetComponent<InputReader>();

        input.PauseEvent += TriggerPause;
    }

    void OnDestroy()
    {
        input.PauseEvent -= TriggerPause;

        soundSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    // This is kinda ugly but it works...avoids manual reference in Inspector
    void SetInitialVolumes()
    {
        soundSlider = GameObject.Find("SoundSlider").GetComponent<Slider>();
        musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();

        if (soundSlider != null)
        {
            soundSlider.value = DataManager.Instance.SoundVolume;
            soundSlider.onValueChanged.AddListener(SetSoundVolume);
        }

        if (musicSlider != null)
        {
            Debug.Log("!!!DATA MANAGER VOLUME IS " + DataManager.Instance.MusicVolume + "!!!");
            Debug.Log("!!!AUDIO MANAGER VOLUME IS " + AudioManager.instance.MusicVolume + "!!!");
            musicSlider.value = DataManager.Instance.MusicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

    }

    void FindButtons()
    {
        resumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();
        if (resumeButton != null) resumeButton.onClick.AddListener(TriggerPause);
        restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        if (quitButton != null) quitButton.onClick.AddListener(ReturnToMenu);
    }

    public void TriggerPause()
    {
        if (isPaused)
        {
            EventManager.TriggerEvent<CursorLockEvent, bool>(true);
            pauseCanvas.enabled = false;
            Time.timeScale = 1f;
            isPaused = false;
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
            //PlayConfirmSound();
        }
        else
        {
            EventManager.TriggerEvent<CursorLockEvent, bool>(false);
            //PlayConfirmSound();
            resumeButton.Select();
            pauseCanvas.enabled = true;
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void PlayConfirmSound()
    {
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(Camera.main.transform.position, confirmSoundName);
    }

    public void SetSoundVolume(float value)
    {
        // Debug.Log("Sound volume is now " + value);
        AudioManager.instance.SoundVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        // Debug.Log("Music volume is now " + value);
        AudioManager.instance.MusicVolume = value;
    }

    public void RestartLevel()
    {
        TriggerPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        TriggerPause();
        DataManager.Instance.CurrentDay = Day.MONDAY;
        DataManager.Instance.Lives = 9;
        DataManager.Instance.Catnip = 0;
        SceneManager.LoadScene("MainMenu");
    }

}
