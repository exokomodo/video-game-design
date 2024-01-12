using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemController : MonoBehaviour
{
    #region Unity lifecycle
    private void Update()
    {
        if (Input.GetKeyUp(InputMap.HardQuit))
        {
            Application.Quit();
        }
    }
    #endregion
}
