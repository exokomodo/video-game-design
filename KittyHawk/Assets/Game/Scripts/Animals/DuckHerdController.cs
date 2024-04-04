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
                StartCoroutine(Quack(anim));
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

    private IEnumerator Quack(Animator anim)
    {
        anim.SetBool("Talk", true);
        yield return new WaitForSeconds(Random.Range(0.2f, 1f));
    }
}
