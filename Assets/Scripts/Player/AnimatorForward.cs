using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorForward : MonoBehaviour
{
    private Character parentCharController;

    private void Awake()
    {
        parentCharController = transform.parent.GetComponent<Character>();
        if (parentCharController == null) Debug.LogError("Parent has no character controller attached!");
    }

    public void OnAnimatorMove()
    {
        if (parentCharController != null) parentCharController.OnAnimatorMove();
    }
}
