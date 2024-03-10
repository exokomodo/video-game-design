using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sets up the level (audio & fade-in)
/// TODO: keep track of objectives
/// Author: Calvin
/// </summary>
public class LevelManager : MonoBehaviour
{

    [SerializeField]
    Canvas canvas;
    [SerializeField]
    string musicName = "MainGameMusic";

    Animator anim;

    void Start()
    {
        anim = canvas.GetComponentInChildren<Animator>();
        anim.SetTrigger("FadeIn");

        EventManager.TriggerEvent<MusicEvent, string>(musicName);

        EventManager.StartListening<PlayerDeathEvent>(OnPlayerDie);
    }

    private void OnDestroy()
    {
        EventManager.StopListening<PlayerDeathEvent>(OnPlayerDie);
    }

    private void OnPlayerDie()
    {
        StartCoroutine(ReloadLevel());
    }

    public IEnumerator ReloadLevel()
    {
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
