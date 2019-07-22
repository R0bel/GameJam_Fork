using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class RandomIdleStateMachine : StateMachineBehaviour
{
    private int[] stateArray = new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 2, 2, 2 };
    private int randomState = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("+");
        randomState = stateArray[Random.Range(0, stateArray.Length)];
        animator.SetInteger("RandomIdleState", randomState);
    }
}
