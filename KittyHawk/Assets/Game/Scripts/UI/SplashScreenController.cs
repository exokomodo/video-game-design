using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
