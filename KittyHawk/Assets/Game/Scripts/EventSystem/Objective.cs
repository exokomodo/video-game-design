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
    public string ObjectiveName;
    public bool Required = true;
    public ObjectiveStatus Status {
        get { return _status; }
        set {
            switch (value) {
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
    private GameObject marker;
    private ObjectiveStatus _status;

    private void PlaceMarker() {
        // Debug.Log("Objective > PLACE MARKER");
        marker = Instantiate(MarkerPrefab, MarkerLocation, Quaternion.identity);
    }
}


