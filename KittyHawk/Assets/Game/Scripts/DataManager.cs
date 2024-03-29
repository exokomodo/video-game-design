using UnityEngine;

public enum Day { MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY};

public class DataManager : MonoBehaviour
{

    public Day CurrentDay;

    public int Lives;
    public int Catnip;

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

        SoundVolume = 1f;
        MusicVolume = 1f;
    }

    void Start()
    {
        PlayerInventory.OnLivesChanged += UpdateLives;
        PlayerInventory.OnCatnipChanged += UpdateCatnip;

        EventManager.StartListening<VolumeChangeEvent, float>(UpdateSoundVolume);
        EventManager.StartListening<MusicVolumeChangeEvent, float>(UpdateMusicVolume);
    }

    private void OnDestroy()
    {
        PlayerInventory.OnLivesChanged -= UpdateLives;
        PlayerInventory.OnCatnipChanged -= UpdateCatnip;

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

    private void UpdateSoundVolume(float volume)
    {
        SoundVolume = volume;
    }

    private void UpdateMusicVolume(float volume)
    {
        MusicVolume = volume;
    }

}
