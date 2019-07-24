using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTTasks : MonoBehaviour
{

    private Animator anim;
    private NavMeshAgent agent;
    private Task task;

    void Awake()
    {
        anim = transform.GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        agent.updatePosition = false;
    }


    [Task]
    void IsMoving()
    {
        task = Task.current;
        task.debugInfo = agent.remainingDistance.ToString();
        bool shouldMove = agent.remainingDistance > agent.stoppingDistance;
        if (shouldMove)
        {
            task.Succeed();
        }
        else
        {
            task.Fail();
        }
    }

    [Task]
    void ShouldMove(bool isMoving)
    {
        task = Task.current;
        anim.SetBool("OnWalk", isMoving);
        task.Succeed();
    }

    [Task]
    void PlayRunAnim()
    {
        task = Task.current;
        anim.SetBool("Running", true);
        anim.SetFloat("RunSpeed", agent.velocity.magnitude * 2);
        task.Succeed();
    }
}
