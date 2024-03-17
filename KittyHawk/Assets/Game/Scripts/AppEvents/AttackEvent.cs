using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AttackEvents are generally triggered by AttackEventBehaviors
/// attached to Animation Controller states
/// Author: Geoffrey Roth
/// </summary>
public class AttackEvent: UnityEvent<string, float, Collider> {

    public const string ATTACK_BEGIN = "AttackBegin";
    public const string ATTACK_END = "AttackEnd";
    public const string ATTACK_HIT = "AttackHit";
}
