using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    #region Unity lifecycle
    private void Start()
    {
        this.lockCursor();
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
            this.unlockCursor();
        }
        else
        {
            this.lockCursor();
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyUp(InputMap.ToggleCursor))
        {
            this.toggleLock();
        }
    }
    #endregion
}
