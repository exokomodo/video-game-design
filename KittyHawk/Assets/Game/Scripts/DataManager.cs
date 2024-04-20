using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to keep track of important game data between scenes
/// Author: Calvin Ferst
/// </summary>

public enum Day { MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY};

public class DataManager : MonoBehaviour
{

    public Day CurrentDay = Day.MONDAY;

    public int Lives = 9;
    public int Catnip = 0;

    public int Chickens = 0;

    public int Bunnies = 0;
    public int BunniesTotal = 0;

    public float SoundVolume = 1f;
    public float MusicVolume = 1f;

    public static DataManager Instance;

    const string DIALOGUE_MONDAY = "ChickenLevelDone";
    const string DIALOGUE_TUESDAY = "DuckLevelDone";
    const string DIALOGUE_WEDNESDAY = "CowObjectiveComplete";
    const string DIALOGUE_THURSDAY = "BunnyCompleteDialogueDuck";
    const string DIALOGUE_FRIDAY = "HorseComplete";
    const string LEVEL_MONDAY = "PaulScene";
    const string LEVEL_TUESDAY = "DuckIntro";
    const string LEVEL_WEDNESDAY = "BenScene";
    const string LEVEL_THURSDAY = "LevelBunnyHop";
    const string LEVEL_FRIDAY = "HorseLevel";
    const string MAIN_MENU = "MainMenu";

    // for level jumping
    readonly KeyCode[] LEVEL_JUMP_COMMAND = { KeyCode.L, KeyCode.E, KeyCode.V, KeyCode.E, KeyCode.L };
    int index = 0;
    bool levelEntered = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
    }

    void Start()
    {
        PlayerInventory.OnLivesChanged += UpdateLives;
        PlayerInventory.OnCatnipChanged += UpdateCatnip;
        PlayerInventory.OnBunniesChanged += UpdateBunnies;
        PlayerInventory.OnBunniesTotalChanged += UpdateBunniesTotal;

        EventManager.StartListening<VolumeChangeEvent, float>(UpdateSoundVolume);
        EventManager.StartListening<MusicVolumeChangeEvent, float>(UpdateMusicVolume);
    }

    private void OnDestroy()
    {
        PlayerInventory.OnLivesChanged -= UpdateLives;
        PlayerInventory.OnCatnipChanged -= UpdateCatnip;
        PlayerInventory.OnBunniesChanged -= UpdateBunnies;
        PlayerInventory.OnBunniesTotalChanged -= UpdateBunniesTotal;

        EventManager.StopListening<VolumeChangeEvent, float>(UpdateSoundVolume);
        EventManager.StopListening<MusicVolumeChangeEvent, float>(UpdateMusicVolume);
    }

    private void UpdateLives(int lives)
    {
        Lives = lives;
    }

    private void UpdateCatnip(int catnip)
    {
        Catnip = catnip;
    }

    private void UpdateBunnies(int bunnies)
    {
        Bunnies = bunnies;
    }

    private void UpdateBunniesTotal(int bunniesTotal)
    {
        BunniesTotal = bunniesTotal;
    }

    private void UpdateSoundVolume(float volume)
    {
        SoundVolume = volume;
    }

    private void UpdateMusicVolume(float volume)
    {
        MusicVolume = volume;
    }

    #region Level Jumping

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (levelEntered)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Instance.CurrentDay = Day.MONDAY;
                    SceneManager.LoadScene(LEVEL_MONDAY);

                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Instance.CurrentDay = Day.TUESDAY;
                    SceneManager.LoadScene(LEVEL_TUESDAY);

                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Instance.CurrentDay = Day.WEDNESDAY;
                    SceneManager.LoadScene(LEVEL_WEDNESDAY);

                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Instance.CurrentDay = Day.THURSDAY;
                    SceneManager.LoadScene(LEVEL_THURSDAY);

                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    Instance.CurrentDay = Day.FRIDAY;
                    SceneManager.LoadScene(LEVEL_FRIDAY);

                }

                index = 0;
                levelEntered = false;
            }
            else
            {
                if (Input.GetKeyDown(LEVEL_JUMP_COMMAND[index]))
                {
                    index++;
                }
                else
                {
                    index = 0;
                    levelEntered = false;
                }
            }
        }

        if (index == LEVEL_JUMP_COMMAND.Length)
        {
            levelEntered = true;
        }
    }

    #endregion

}
