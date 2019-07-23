using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioExampleScene : SceneMonoBehaviour
{
    public override void OnStart(GameManager _manager)
    {
        Debug.Log($"{this.GetType().Name} initialized");
    }
}
