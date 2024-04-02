using UnityEngine;

/// <summary>
/// CursorManager manages the cursor state of the game
/// Author: James Orson
/// </summary>

public class CursorManager : MonoBehaviour
{
    private static CursorManager _instance;
    public static CursorManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(CursorManager)) as CursorManager;
                if (!_instance)
                {
                    Debug.LogError($"There needs to be one active {nameof(CursorManager)} script on a GameObject in your scene.");
                }
            }
            return _instance;
        }
    }

    #region Unity lifecycle
    private void Start()
    {
        EventManager.StartListening<CursorLockEvent, bool>(OnCursorLockEvent);
        EventManager.StartListening<DialogueOpenEvent, Vector3, string>(OnDialogueOpenEvent);
        EventManager.StartListening<DialogueCloseEvent, string>(OnDialogueCloseEvent);
    }

    private void OnCursorLockEvent(bool shouldLock)
    {
        if (shouldLock)
        {
            lockCursor();
        }
        else
        {
            unlockCursor();
        }
    }

    private void OnDialogueOpenEvent(Vector3 position, string dialogue)
    {
        EventManager.TriggerEvent<CursorLockEvent, bool>(false);
    }

    private void OnDialogueCloseEvent(string dialogue)
    {
        EventManager.TriggerEvent<CursorLockEvent, bool>(true);
    }
    
    private void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void unlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    #endregion
}
