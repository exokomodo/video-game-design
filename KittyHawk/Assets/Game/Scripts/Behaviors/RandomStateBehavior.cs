using UnityEngine;
using UnityEngine.Animations;

public class RandomStateBehavior : StateMachineBehaviour
{
    [Tooltip("Number of animations to randomize")]
    public int range = 0;

    private static int RandomHash = Animator.StringToHash("RandomInt");
    private static int randInt = -1;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        Debug.Log("RandomStateBehavior: OnStateEnter");
        SetRandomInt();
        animator.SetInteger(RandomHash, randInt);
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
