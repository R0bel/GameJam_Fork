using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTTasks : MonoBehaviour
{
    [SerializeField]
    private Transform[] targets;
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

    [Task]
    void PlayPrayAnim()
    {
        task = Task.current;
        anim.SetTrigger("Praying");
        task.Succeed();
    }

    [Task]
    void PlayDeathAnim()
    {
        task = Task.current;
        anim.SetTrigger("IsDead");
        task.Succeed();
    }

    [Task]
    void NearPlayer(float minDist)
    {
        task = Task.current;
        for (int i = 0; i < targets.Length; i++)
        {
            float dist = Vector3.Distance(targets[i].position, transform.position);
            Debug.Log(dist);
            if (dist <= minDist)
            {
                task.Succeed();
                break;
            }
            else
            {
                task.Fail();
            }
        }

    }


    [Task]
    void RandomPoint(int range, float minDist)
    {
        task = Task.current;
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                float dist = Vector3.Distance(hit.position, transform.position);
                if (dist >= minDist)
                {
                    agent.destination = hit.position;
                    task.Succeed();
                }
            }
        }
        task.Fail();
    }
}