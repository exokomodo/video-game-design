using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    public Vector2 MovementValue { get; private set; }
    public event Action AttackRightEvent;
    public event Action AttackFrontEvent;
    public event Action AttackLeftEvent;
    public event Action JumpEvent;
    public event Action MeowEvent;
    public event Action RunEvent;
    public event Action RunStopEvent;
    private Controls controls;
    private void Start()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);

        controls.Player.Enable();
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }

    public void OnAttackRight(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        AttackRightEvent?.Invoke();
    }

    public void OnAttackFront(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        AttackFrontEvent?.Invoke();
    }

    public void OnAttackLeft(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        AttackLeftEvent?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        JumpEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {

    }

    public void OnMeow(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        MeowEvent?.Invoke();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (!context.performed) {
            RunStopEvent?.Invoke();
            return;
        }
        RunEvent?.Invoke();
    }
}
