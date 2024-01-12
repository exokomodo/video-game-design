using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodCameraController : MonoBehaviour
{
    #region Inputs
    public float speed = 15.0f;
    public float sensitivity = 50.0f;
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // SOURCE: https://medium.com/@mikeyoung_97230/creating-a-simple-camera-controller-in-unity3d-using-c-ec1a79584687

        // NOTE: Move the camera forward, backward, left, and right
        transform.position += transform.forward * Input.GetAxis("Vertical") * this.speed * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * this.speed * Time.deltaTime;

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
