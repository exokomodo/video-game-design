using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    #region Inputs
    public float speed = 15.0f;
    public float sprintFactor = 3.0f;
    #endregion
    
    void Update()
    {
        // SOURCE: https://medium.com/@mikeyoung_97230/creating-a-simple-camera-controller-in-unity3d-using-c-ec1a79584687

        // NOTE: Move the player forward, backward, left, and right
        var acceleration = this.speed;
        if (InputMap.IsSprinting)
        {
            acceleration *= this.sprintFactor;
        }
        transform.position += transform.forward * Input.GetAxis("Vertical") * acceleration * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * acceleration * Time.deltaTime;
        if (InputMap.IsJumping)
        {
                
        }
    }
}
