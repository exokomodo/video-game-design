using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseController : MonoBehaviour
{
    #region Unity Components
    private Animator _animator;
    #endregion

    public float Velocity = 0f;
    public const float VelocityScale = 5f;

    public Vector3 ActualForward => -transform.right;

    #region Unity hooks
    private void Start()
    {
        this._animator = GetComponent<Animator>();        
    }

    private void FixedUpdate()
    {
        this.transform.position += (
            ActualForward *
            Velocity *
            VelocityScale *
            Time.fixedDeltaTime
        );
    }

    private void Update()
    {
        this._animator.SetFloat("Speed", this.Velocity);
    }
    #endregion
}
