using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Saddle A component that can be added to a GameObject to make it saddled, making the object into a hat
/// Author: James Orson
/// </summary>
public class Saddle : MonoBehaviour
{
    #region Public fields
    public Vector3 SaddleHeight = Vector3.zero;
    [KittyHawk.Attributes.TagSelector]
    public string RiderTag = "Player";
    #endregion

    #region Protected properties
    protected GameObject _rider = null;
    #endregion

    #region Unity hooks
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag(RiderTag))
        {
            Debug.Log("Saddle enter");
            Debug.Log("Rider entered");
            _rider = GameObject.FindGameObjectsWithTag("Player")
                .Where(x => x.GetComponent<PlayerController>()).FirstOrDefault();
            EventManager.TriggerEvent<RiderEnterEvent, Saddle, GameObject>(
                this,
                _rider);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(SaddleHeight, 0.1f);
    }

    private void Update()
    {
        if (_rider != null)
        {
            if (InputMap.IsJumping)
            {
                EventManager.TriggerEvent<RiderExitEvent, Saddle, GameObject>(
                    this,
                    _rider);
                _rider = null;
            }
            else
            {
                _rider.transform.position = transform.position + SaddleHeight;
            }
        }
    }

    private void Start()
    {
        Debug.Assert(GetComponent<Rigidbody>() != null, "Saddle must have a rigidbody");
        Debug.Assert(GetComponents<Collider>().Length > 0, "Saddle must at least one collider");
    }
    #endregion
}
