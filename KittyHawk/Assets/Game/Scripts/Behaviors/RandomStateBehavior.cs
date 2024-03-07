using UnityEngine;

/// <summary>
/// Attach RandomStateBehavior to a state to generate a new value for the
/// Animation Controller parameter RandomInt.
/// Useful for randomizing which animation state plays based on a transition condition.
/// Author: Geoffrey Roth
/// </summary>
public class RandomStateBehavior : StateMachineBehaviour
{
    [Tooltip("Number of animations to randomize")]
    public int range = 0;

    private static int RandomHash = Animator.StringToHash("RandomInt");
    private static int randInt = -1;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        SetRandomInt();
        animator.SetInteger(RandomHash, randInt);
        Debug.Log("RandomStateBehavior: RandomInt: " + randInt);
    }

    private int SetRandomInt()
    {
        int rand = (int) Mathf.Floor(Random.Range(0, range));
        while (rand == randInt) {
            rand = (int) Mathf.Floor(Random.Range(0, range));
        }
        randInt = rand;
        return randInt;
    }
}
