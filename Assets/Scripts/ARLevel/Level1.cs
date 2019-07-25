using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

[Serializable]
public class Level1 : ARLevel
{

    [SerializeField]
    private Character character;

    protected override void OnLevelStart()
    {
        Debug.Log("Started Level: " + gameObject.name);



    }

    protected override void OnLevelStop()
    {
        if (gameManager.Char.ActiveCharacter != null)
        {
            PhotonView view = gameManager.Char.ActiveCharacter.GetComponent<PhotonView>();
            PhotonNetwork.Destroy(view);
        }
        Debug.Log("Stopped Level: " + gameObject.name);
    }

    public override void SpawnCharacter(string _playerModelName)
    {
        if (gameManager.Network.InRoom)
        {
            if (gameManager.Network.IsMasterClient)
            {
                foreach (GameObject hoomanSpawn in hoomanSpawns)
                {
                    GameObject hooman = PhotonNetwork.InstantiateSceneObject("Hooman Variant", hoomanSpawn.transform.position, hoomanSpawn.transform.rotation, 0);
                    BTTasks hoomanBehaviour = hooman.GetComponent<BTTasks>();
                }
            }            

            GameObject currentChar = PhotonNetwork.Instantiate(_playerModelName, playerSpawn.transform.position, playerSpawn.transform.rotation, 0);
            gameManager.Char.ActiveCharacter = currentChar.GetComponent<Character>();          

        }
    }
}
