using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
  public Animator anim;

  public PlayerStateMachine FSM;

  public Vector3 velocity;

  public bool isGrounded = true;


  public void Awake() {

  }

  public void Move(Vector3 motion)
  {

  }
}
