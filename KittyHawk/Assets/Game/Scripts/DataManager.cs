using UnityEngine;

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

}
