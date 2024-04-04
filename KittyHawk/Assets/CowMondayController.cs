using UnityEngine;

public class CowMondayController : MonoBehaviour
{

    private Animator anim;
    private bool alreadyTalked;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !alreadyTalked)
        {
            anim.SetBool("jumping", true);
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowDialoguePaul");
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