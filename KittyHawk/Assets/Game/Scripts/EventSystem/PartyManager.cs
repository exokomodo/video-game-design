using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the party scne introducing trash level
/// Author: Calvin Ferst
/// </summary>
public class PartyManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] geese;
    [SerializeField]
    float delay = 4f;

    private void Start()
    {
        StartCoroutine(Party());
    }

    IEnumerator Party()
    {
        yield return new WaitForSeconds(4f);

        foreach (GameObject goose in geese)
        {
            Animator anim = goose.GetComponentInChildren<Animator>();

            if (anim != null)
                anim.SetBool("PartyTime", true);
        }
    }

}
