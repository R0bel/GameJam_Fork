using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SceneLoadingOrder(Priority = 0)]
public abstract class SceneMonoBehaviour : MonoBehaviour, IManagedBehaviour
{
    public abstract void OnStart(GameManager _manager);
}
