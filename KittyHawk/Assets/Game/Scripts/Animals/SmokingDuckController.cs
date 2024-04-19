using UnityEngine;

/// <summary>
/// Basic script to set up smoking duck
/// Author: Calvin Ferst
/// </summary>
public class SmokingDuckController : MonoBehaviour
{

    [SerializeField]
    string audioName = "Quack";

    protected Animator anim;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        ToggleTalking(false);
    }

    public void ToggleTalking(bool talk)
    {
        anim.SetBool("Talk", talk);
    }

    public void Quack()
    {
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, audioName);
    }
}
