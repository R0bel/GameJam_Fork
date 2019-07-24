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

        // Character currentChar = Instantiate(character, Vector3.zero, Quaternion.identity, transform);
        // gameManager.Char.ActiveCharacter = currentChar;
    }

    public override void SpawnCharacter()
    {
        if (gameManager.Network.InRoom)
        {
            GameObject currentChar = PhotonNetwork.Instantiate("CowPlayer", transform.position, Quaternion.identity, 0);
            gameManager.Char.ActiveCharacter = currentChar.GetComponent<Character>();
        }
    }
}
