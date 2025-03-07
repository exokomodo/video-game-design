using KittyHawk.Extensions;
using UnityEngine;

/// <summary>
/// ScriptableObject for creating objective assets
/// Author: Calvin Ferst
/// Markers added by: Geoffrey Roth
/// </summary>
public enum ObjectiveStatus { NotStarted, InProgress, Failed, Completed }

[CreateAssetMenu(fileName = "New Objective", menuName = "Pentaclaw/Objective")]
public class Objective : ScriptableObject
{
    private GameObject marker;
    private ObjectiveStatus _status;

    public string ObjectiveName;
    public bool Required = true;
    public ObjectiveStatus Status {
        get { return _status; }
        set {
            switch (value) {
                case ObjectiveStatus.NotStarted:
                    if (marker) marker.SetActive(false);
                    break;
                case ObjectiveStatus.InProgress:
                    PlaceMarker();
                    break;
                case ObjectiveStatus.Completed:
                case ObjectiveStatus.Failed:
                    if (marker) Destroy(marker);
                    break;
            }
            _status = value;
        }
    }
    public bool ShowMarker = false;
    public Vector3 MarkerLocation;
    public GameObject MarkerPrefab;
    [HideInInspector]
    public Transform FollowTarget;
    public Transform transform {
        get {
            return marker?.transform;
        }
    }

    public Vector3 Scale = Vector3.zero;

    private void PlaceMarker() {
        if (ShowMarker && MarkerPrefab != null) {
            if (marker != null) {
                marker.SetActive(true);
                return;
            }
            marker = Instantiate(MarkerPrefab, MarkerLocation, Quaternion.identity);
            ArrowMarker am = marker.GetComponentInChildren<ArrowMarker>();
            if (FollowTarget) {
                am.FollowTarget = FollowTarget;
            }
            if (Scale != Vector3.zero) am.gameObject.transform.localScale = Scale;
        }
    }
}


