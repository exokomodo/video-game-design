using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public PlayerController Controller { get; private set; }
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public float FreeMovementSpeed { get; private set; }
    [field: SerializeField] public float RunningSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }

    public enum StateEnum
    {
        IDLE,
        SIT,
        LIE,
        WALK,
        RUN,
        JUMP,
        FALL,
        CLIMB,
        CRAWL,
        SWIM,
        ATTACK,
        DIE
    }

    public Transform MainCameraTransform { get; private set; }

    private void Start()
    {
        MainCameraTransform = Camera.main.transform;
        SwitchState(new PlayerIdleState(this));
    }
}
