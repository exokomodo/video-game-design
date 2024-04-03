using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckHerdController : MonoBehaviour
{

    Animator[] anims;

    void Start()
    {
        anims = GetComponentsInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (Animator anim in anims)
            {
                anim.SetBool("Talk", true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (Animator anim in anims)
            {
                anim.SetBool("Talk", false);
            }
        }
    }
}
