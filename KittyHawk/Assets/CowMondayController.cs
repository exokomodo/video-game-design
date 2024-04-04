using UnityEngine;

public class CowMondayController : MonoBehaviour
{

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            anim.SetBool("jumping", true);
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowDialoguePaul");
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