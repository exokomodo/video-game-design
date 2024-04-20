using System.Collections;
using UnityEngine;

/// <summary>
/// Script controlling behavior of ducks in response to Kitty Hawk
/// Author: Calvin Ferst
/// </summary>
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
