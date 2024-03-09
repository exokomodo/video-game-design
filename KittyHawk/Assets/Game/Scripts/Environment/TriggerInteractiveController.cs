using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General script that reacts to player's presence and plays audio
/// Author: Calvin Ferst
/// </summary>
public class TriggerInteractiveController : MonoBehaviour
{

    [SerializeField]
    string audioName;

    Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            anim.SetTrigger("KHNearby");
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, audioName);
        }
    }

}
