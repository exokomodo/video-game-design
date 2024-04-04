using UnityEngine;

public class CowDuckController : MonoBehaviour
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
            EventManager.TriggerEvent<DialogueOpenEvent, Vector3, string>(transform.position, "CowDuckDialogue");
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
