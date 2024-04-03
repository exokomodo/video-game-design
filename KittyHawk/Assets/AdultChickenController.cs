using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AdultChickenController : MonoBehaviour
{

    Animator anim;
    private bool alreadyTalked;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !alreadyTalked)
        {
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "ChickenDialogue");
            alreadyTalked = true;
        }
    }
}
