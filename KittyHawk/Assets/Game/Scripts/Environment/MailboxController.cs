using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for environment interactive, launching letter from mailbox
/// Author: Calvin Ferst
/// </summary>
public class MailboxController : MonoBehaviour
{

    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    GameObject letterPrefab;
    [SerializeField]
    float minLaunchForce = 5f;
    [SerializeField]
    float maxLaunchForce = 20f;

    public void LaunchLetter()
    {
        GameObject letter = Instantiate(letterPrefab, spawnPoint);
        letter.GetComponent<Rigidbody>().AddForce(Random.Range(minLaunchForce, maxLaunchForce), 0, 0);
    }
}
