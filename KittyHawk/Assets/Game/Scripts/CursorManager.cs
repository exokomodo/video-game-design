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
        lockCursor();
    }
    
    public void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void unlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void toggleLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            unlockCursor();
        }
        else
        {
            lockCursor();
        }
    }
    
    private void Update()
    {
        if (InputMap.ShouldToggleCursor)
        {
            toggleLock();
        }
    }
    #endregion
}
