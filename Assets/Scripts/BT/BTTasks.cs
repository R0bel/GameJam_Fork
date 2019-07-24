using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTTasks : MonoBehaviour
{
    [SerializeField]
    Animator anim;
    [SerializeField]
    NavMeshAgent agent;

    void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        agent.updatePosition = false;
    }


    [Task]
    void ShouldMove()
    {
        Task task = Task.current;
        bool shouldMove = agent.remainingDistance > agent.radius - agent.stoppingDistance;
        if (shouldMove)
        {
            task.Succeed();
        }
        else
        {
            task.Fail();
        }
    }
}
