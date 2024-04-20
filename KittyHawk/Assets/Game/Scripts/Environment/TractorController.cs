using UnityEngine;

/// <summary>
/// Logic for tractor in goose party scene
/// Author: Calvin Ferst
/// </summary>

public class TractorController : MonoBehaviour
{

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Run()
    {
        anim.SetTrigger("Crash");
    }

}
