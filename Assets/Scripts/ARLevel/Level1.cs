using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Level1 : ARLevel
{
    [SerializeField]
    private Character character;

    protected override void OnLevelStart()
    {
        Debug.Log("Started Level: " + gameObject.name);
        //character.gameObject.SetActive(true);
        Character currentChar = Instantiate(character, Vector3.zero, Quaternion.identity, transform);
        gameManager.Char.ActiveCharacter = currentChar;
    }
}
