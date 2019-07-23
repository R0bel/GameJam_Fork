using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManagedBehaviour
{
    /// <summary>
    /// Called after GameManager instance is ready to use
    /// </summary>
    /// <param name="_manager"></param>
    void OnStart(GameManager _manager);
}
