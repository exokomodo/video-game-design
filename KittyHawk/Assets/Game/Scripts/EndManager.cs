using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{
    [SerializeField]
    float textWaitDuration = 5f;
    [SerializeField]
    GameObject fadeImage;
    [SerializeField]
    GameObject endText;

    private Animator anim;

    private float startingVolume;

    void Start()
    {
        startingVolume = AudioManager.instance.MusicVolume;

        endText.SetActive(false);
        anim = fadeImage.GetComponent<Animator>();
        StartCoroutine(DisplayEndText());
    }

    public void MainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        LowerVolume();
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("FadeOut");
        LowerVolume();
        yield return new WaitForSeconds(textWaitDuration);
        endText.SetActive(false);
        LowerVolume();
        yield return new WaitForSeconds(2f);
        AudioManager.instance.MusicVolume = startingVolume;
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator DisplayEndText()
    {
        yield return new WaitForSeconds(textWaitDuration);
        endText.SetActive(true);
        yield return new WaitForSeconds(textWaitDuration);
        MainMenu();
    }

    void LowerVolume()
    {
        if (AudioManager.instance.MusicVolume != 0)
        {
            AudioManager.instance.MusicVolume *= 0.8f;
        }
    }

}
