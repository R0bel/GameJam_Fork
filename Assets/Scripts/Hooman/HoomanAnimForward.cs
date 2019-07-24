using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HoomanAnimForward : MonoBehaviour
{
    private Transform parent;
    private NavMeshAgent agent;

    private void Awake()
    {
        parent = transform.parent.transform;
        if (parent == null) Debug.LogError("Parent has no AgentAnimMove controller attached!");
        agent = parent.GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        agent.updatePosition = false;
    }

    public void OnAnimatorMove()
    {
        parent.position = agent.nextPosition;
    }
}
