using System.Collections;
using UnityEngine;

/// <summary>
/// Logic for fixing tractor
/// Author: Calvin Ferst
/// </summary>
public class BrokenTractorController : MonoBehaviour
{

    bool gotHammer;
    bool gotTape;
    bool isFixed;

    void Start()
    {
        EventManager.StartListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);

        gotHammer = false;
        gotTape = false;
        isFixed = false;
    }

    private void OnDestroy()
    {
        EventManager.StopListening<ObjectiveChangeEvent, string, ObjectiveStatus>(OnObjectiveChange);
    }

    void OnObjectiveChange(string name, ObjectiveStatus status)
    {
        if (name == "GetHammer" && status == ObjectiveStatus.Completed)
        {
            gotHammer = true;
        }

        if (name == "GetDuctTape" && status == ObjectiveStatus.Completed)
        {
            gotTape = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gotHammer && gotTape && !isFixed)
        {
            isFixed = true;
            DisableGeese();
            StartCoroutine(RepairTractor());
        }
    }

    private void DisableGeese() {
        GooseAI[] geese = FindObjectsOfType<GooseAI>();
        foreach (GooseAI goose in geese) {
            goose.Disable();
        }
    }

    IEnumerator RepairTractor()
    {
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "TapeFix");
        yield return new WaitForSeconds(1f);
        EventManager.TriggerEvent<AudioEvent, Vector3, string>(transform.position, "Hammering");
        yield return new WaitForSeconds(1f);
        EventManager.TriggerEvent<ObjectiveChangeEvent, string, ObjectiveStatus>("FixTractor", ObjectiveStatus.Completed);
    }

}
