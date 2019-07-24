using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ARLevel : MonoBehaviour
{
    protected GameManager gameManager;

    [SerializeField]
    protected GameObject playerSpawn;
    [SerializeField]
    protected GameObject[] hoomanSpawns; 

    [SerializeField]
    protected string imageName;

    public string ImageName
    {
        get
        {
            return imageName;
        }
    }

    public void StartLevel()
    {
        gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            // trigger level start event
            gameManager.Events.OnARLevelStarted(this);

            if (!gameManager.Network.IsConnectedAndReady)
                gameManager.Network.ConnectToMasterServer();
            OnLevelStart();
        }        
    }

    public void StopLevel()
    {
        gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            // trigger level stop event
            gameManager.Events.OnARLevelStopped(this);

            if (gameManager.Network.InRoom)
                gameManager.Network.LeaveRoom();
            OnLevelStop();
        }        
    }

    protected virtual void OnLevelStart()
    {

    }

    protected virtual void OnLevelStop()
    {

    }

    public virtual void SpawnCharacter(string _playerModelName)
    {

    }
}
