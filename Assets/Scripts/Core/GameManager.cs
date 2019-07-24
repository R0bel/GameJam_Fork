using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public EventManager Events { get; private set; }
    public AudioManager Audio { get; private set; }
    public CharacterManager Char { get; private set; }
    public TrackedImageInfoManager AR { get; private set; }
    public NetworkManager Network { get; private set; }

    private List<MonoBehaviour> managedBehaviours;

    void Awake()
    {
        // singelton pattern
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        // get components
        Events = GetComponentInChildren<EventManager>();
        Audio = GetComponentInChildren<AudioManager>();
        Char = GetComponentInChildren<CharacterManager>();
        AR = GetComponentInChildren<TrackedImageInfoManager>();
        Network = GetComponentInChildren<NetworkManager>();

        managedBehaviours = new List<MonoBehaviour>();
        Debug.LogWarning("GameManager initialized");
    }

    private void Start()
    {
        // start ManagedMonoBehaviours
        managedBehaviours.Clear();
        MonoBehaviour[] behaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IManagedBehaviour) managedBehaviours.Add(behaviour);
        }
        managedBehaviours = managedBehaviours.OrderBy(b => GetLoadingPriority(b)).ToList();        

        foreach (IManagedBehaviour behaviour in managedBehaviours)
        {
            behaviour.OnStart(this);
        }
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        // Trigger startup event
        Events.OnGameStartupFinished();
    }

    private int GetLoadingPriority(Behaviour _behaviour)
    {
        int priority = 1;
        if (_behaviour.gameObject.scene.buildIndex == -1)
        {
            // if behaviour is inside GameManager
            priority = -1;
        } else
        {
            SceneLoadingOrder[] customAttributes = (SceneLoadingOrder[])_behaviour
                .GetType().GetCustomAttributes(typeof(SceneLoadingOrder), true);

            if (customAttributes.Length > 0)
            {
                SceneLoadingOrder sceneLoadingAttributes = customAttributes[0];
                priority = sceneLoadingAttributes.Priority;
            }
        }        
        return priority;
    }

    private void OnActiveSceneChanged(Scene _current, Scene _next)
    {
        Debug.LogWarning($"GameManager initializing Scene: {_next.name}");

        // init ManagedMonoBehaviours that are not inside DontDestroyOnLoad
        managedBehaviours.Clear();
        MonoBehaviour[] behaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IManagedBehaviour) managedBehaviours.Add(behaviour);
        }
        managedBehaviours = managedBehaviours.OrderBy(b => GetLoadingPriority(b)).ToList();

        foreach (MonoBehaviour behaviour in managedBehaviours)
        {
            if (behaviour.gameObject.scene.buildIndex != -1 && behaviour is IManagedBehaviour)
            {                
                ((IManagedBehaviour)behaviour).OnStart(this);
            }            
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }
}
