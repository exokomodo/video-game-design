using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Helper class for animating splash screen
/// Author: Calvin Ferst
/// </summary>
public class SplashScreenController : MonoBehaviour
{
    [SerializeField]
    float fadeTime = 4f;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeTime);
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
    }

    /*
    public void OnFadeComplete()
    {
        SceneManager.LoadScene("MainMenu");
    } */
}
