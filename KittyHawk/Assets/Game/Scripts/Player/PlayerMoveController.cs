using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    #region Inputs
    public float speed = 15.0f;
    public float sprintFactor = 3.0f;

    private Animator anim;
    #endregion

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // SOURCE: https://medium.com/@mikeyoung_97230/creating-a-simple-camera-controller-in-unity3d-using-c-ec1a79584687

        // NOTE: Move the player forward, backward, left, and right
        var acceleration = this.speed;
        if (InputMap.IsSprinting)
        {
            acceleration *= this.sprintFactor;
        }

        var movementForward = Input.GetAxis("Vertical");
        var movementRight = Input.GetAxis("Horizontal");

        transform.position += transform.forward * movementForward * acceleration * Time.deltaTime;
        transform.position += transform.right * movementRight * acceleration * Time.deltaTime;
        if (InputMap.IsJumping)
        {
            anim.SetTrigger("Jump");
        }

        // animation

        anim.SetFloat("Speed", movementForward);

    }
}
