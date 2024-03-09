using UnityEngine;

/// <summary>
/// CursorManager manages the cursor state of the game
/// Author: James Orson
/// </summary>

public class CursorManager : MonoBehaviour
{
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
