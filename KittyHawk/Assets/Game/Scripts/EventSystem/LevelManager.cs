using UnityEngine;

/// <summary>
/// Sets up the level (audio & fade-in)
/// TODO: keep track of objectives
/// Author: Calvin
/// </summary>
public class LevelManager : MonoBehaviour
{

    [SerializeField]
    Canvas canvas;

    Animator anim;

    void Start()
    {
        anim = canvas.GetComponentInChildren<Animator>();
        anim.SetTrigger("FadeIn");

        EventManager.TriggerEvent<MusicEvent, string>("MainGameMusic");
    }

}
