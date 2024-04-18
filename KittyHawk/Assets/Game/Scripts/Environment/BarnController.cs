using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnController : MonoBehaviour
{

    Animator anim;
    bool gotKey;
    bool open;

    private void Start()
    {
        EventManager.StartListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);

        anim = GetComponent<Animator>();
        gotKey = false;
        open = false;
    }

    private void OnDestroy()
    {
        EventManager.StopListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && gotKey && !open)
        {
            open = true;
            anim.SetTrigger("Open");
            EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "BarnDoorsOpen");
            EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>(DuckLevelController.Objectives.BARN, ObjectiveStatus.Completed);
        }
    }

    void OnObjectiveChange(string name, ObjectiveStatus status)
    {
        if (name == "GetBarnKey" && status == ObjectiveStatus.Completed)
        {
            gotKey = true;
        }
    }

}
