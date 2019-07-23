using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ARLevel : MonoBehaviour
{
    protected GameManager gameManager;

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
            OnLevelStart();
        }        
    }

    protected virtual void OnLevelStart()
    {

    }

    public virtual void SpawnCharacter()
    {

    }
}
