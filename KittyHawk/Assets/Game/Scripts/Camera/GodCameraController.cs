using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodCameraController : MonoBehaviour
{
    #region Inputs
    public float sensitivity = 50.0f;
    #endregion
    
    void Update()
    {
        // SOURCE: https://medium.com/@mikeyoung_97230/creating-a-simple-camera-controller-in-unity3d-using-c-ec1a79584687

        // NOTE: Rotate the camera based on the mouse movement
        var mouse = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"));
        
        transform.eulerAngles += new Vector3(
            -mouse.y,
            mouse.x,
            0) * this.sensitivity * Time.deltaTime;
    }
}
