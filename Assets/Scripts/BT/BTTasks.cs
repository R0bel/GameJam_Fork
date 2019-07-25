using Panda;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTTasks : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField]
    private List<Transform> targets;
    private Animator anim;
    private NavMeshAgent agent;
    private Task task;
    private HoomanPhotonControl photonControl;


    void Awake()
    {
        gameManager = GameManager.Instance;
        targets = new List<Transform>();
        anim = transform.GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        photonControl = GetComponent<HoomanPhotonControl>();
        // Don’t update position automatically
        agent.updatePosition = false;

        gameManager.Events.CharacterSpawned += OnCharacterSpawned;
        gameManager.Events.CharacterDespawned += OnCharacterDespawned;
        gameManager.Events.PlayerLeftRoom += OnPlayerCountChanged;
        gameManager.Events.PlayerJoinedRoom += OnPlayerCountChanged;
    }

    private void OnEnable()
    {
        agent.enabled = gameManager.Network.IsMasterClient;

    }

    private void OnPlayerCountChanged(Player _player)
    {
        agent.enabled = gameManager.Network.IsMasterClient;
    }

    private void OnDestroy()
    {
        gameManager.Events.CharacterSpawned -= OnCharacterSpawned;
        gameManager.Events.CharacterDespawned -= OnCharacterDespawned;
        gameManager.Events.PlayerLeftRoom -= OnPlayerCountChanged;
        gameManager.Events.PlayerJoinedRoom -= OnPlayerCountChanged;
    }

    private void OnCharacterSpawned(GameObject _characterObj)
    {
        targets.Clear();
        foreach (Player player in gameManager.Network.RoomPlayers)
        {
            if (player.TagObject != null) targets.Add(((GameObject)player.TagObject).transform);
        }
    }

    private void OnCharacterDespawned(GameObject _characterObj)
    {
        targets.Remove(_characterObj.transform);
    }

    public void GotHit(int _damage)
    {
        photonControl.Health -= _damage;
        Debug.LogWarning(photonControl.Health);
        if (photonControl.Health <= 0)
        {
            
        }
    }


    [Task]
    void AgentOn()
    {
        task = Task.current;
        task.Complete(agent.enabled);
    }

    [Task]
    void IsMoving()
    {
        task = Task.current;
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
        for (int i = 0; i < targets.Count; i++)
        {
            float dist = Vector3.Distance(targets[i].position, transform.position);
            // Debug.Log(dist);
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