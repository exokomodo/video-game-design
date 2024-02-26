using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReceiver : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    // Paul changed this to public to see if it would help with the tire interaction
    public float verticalVelocity;

    public Vector3 Movement => Vector3.up * verticalVelocity;

    private void Update()
    {
        if (verticalVelocity < 0f && controller.isGrounded)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }

    public void Jump(float jumpForce)
    {
        verticalVelocity += jumpForce;
    }
}
