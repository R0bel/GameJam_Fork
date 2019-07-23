using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoomanAnimForward : MonoBehaviour
{
    private AgentAnimMove parentAgentAnimMove;

    private void Awake()
    {
        parentAgentAnimMove = transform.parent.GetComponent<AgentAnimMove>();
        if (parentAgentAnimMove == null) Debug.LogError("Parent has no AgentAnimMove controller attached!");
    }

    public void OnAnimatorMove()
    {
        if (parentAgentAnimMove != null) parentAgentAnimMove.OnAnimatorMove();
    }
}
