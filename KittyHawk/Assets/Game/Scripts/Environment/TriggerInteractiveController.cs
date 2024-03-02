using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
