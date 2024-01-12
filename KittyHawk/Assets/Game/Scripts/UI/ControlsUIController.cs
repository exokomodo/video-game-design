using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsUIController : MonoBehaviour
{
    #region Public components

    public GameObject controlsUi;
    #endregion
    
    #region Unity lifecycle
    private void Start()
    {
        this.controlsUi.SetActive(true);
    }

    private void Update()
    {
        if (InputMap.ShouldToggleControlsUI) {
            this.controlsUi.SetActive(!this.controlsUi.activeInHierarchy);
        }
    }
    #endregion
}
