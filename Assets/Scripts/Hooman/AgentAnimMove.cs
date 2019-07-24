using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentAnimMove : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;

    void Start()
    {
        anim = transform.GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        agent.updatePosition = false;
    }
    /*
    void Update()
    {
        bool shouldMove = agent.remainingDistance > agent.radius - agent.stoppingDistance;

        // Update animation parameters
        anim.SetBool("OnWalk", shouldMove);
        anim.SetBool("Running", true);
        anim.SetFloat("RunSpeed", agent.velocity.magnitude * 0.25f);
        // anim.SetFloat("InputMove", agent.velocity.sqrMagnitude);
    }
    */
    public void OnAnimatorMove()
    {
        // Update position to agent position
        transform.position = agent.nextPosition;
    }
}