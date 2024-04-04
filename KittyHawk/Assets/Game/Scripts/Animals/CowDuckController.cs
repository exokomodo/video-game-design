using UnityEngine;

public class CowDuckController : MonoBehaviour
{

    private Animator anim;
    private bool alreadyTalked;

    private void Start()
    {
        anim = GetComponent<Animator>();
        alreadyTalked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !alreadyTalked)
        {
            anim.SetBool("jumping", true);
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowDuckDialogue");
            alreadyTalked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            anim.SetBool("jumping", false);
        }
    }

}
