using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

[Serializable]
public class Level1 : ARLevel
{

    [SerializeField]
    private Character character;

    protected override void OnLevelStart()
    {
        Debug.Log("Started Level: " + gameObject.name);
        gameManager = GameManager.Instance;

        // connect to Photon Networking
        gameManager.Network.ConnectToMasterserver();
    }

    public override void SpawnCharacter(string _playerModelName)
    {
        if (gameManager.Network.InRoom)
        {

            GameObject currentChar = PhotonNetwork.Instantiate(_playerModelName, transform.position, Quaternion.identity, 0);
            gameManager.Char.ActiveCharacter = currentChar.GetComponent<Character>();
        }
    }
}
