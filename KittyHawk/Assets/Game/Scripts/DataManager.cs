using UnityEngine;

public enum Day { MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY};

public class DataManager : MonoBehaviour
{

    public Day CurrentDay = Day.MONDAY;

    public int Lives;
    public int Catnip;

    public int Bunnies;
    public int BunniesTotal;

    public float SoundVolume;
    public float MusicVolume;

    public static DataManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentDay = Day.MONDAY;

        Lives = 9;
        Catnip = 0;
        Bunnies = 0;
        BunniesTotal = 0;

        SoundVolume = 1f;
        MusicVolume = 1f;
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
