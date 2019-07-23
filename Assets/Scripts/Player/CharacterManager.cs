using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour, IManagedBehaviour
{
    private GameManager gameManager;
    [SerializeField]
    private Character character;

    public void OnStart(GameManager _manager)
    {
        gameManager = _manager;
    }

    public Character ActiveCharacter
    {
        get
        {
            return character;
        }
        set
        {
            if (character != value && gameManager != null)
            {
                gameManager.Events.OnCharacterChanged(value);
            }
            character = value;
        }
    }
}
