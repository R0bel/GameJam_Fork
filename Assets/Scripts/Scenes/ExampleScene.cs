using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExampleScene : SceneMonoBehaviour
{



    public override void OnStart(GameManager _manager)
    {
        Debug.Log($"{this.GetType().Name} initialized");
    }
}
