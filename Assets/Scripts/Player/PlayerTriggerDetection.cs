using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerDetection : MonoBehaviour
{
    private GameManager gameManager;
    private BTTasks hoomanBT;
    [SerializeField]
    private int damage;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hooman")
        {
            hoomanBT = other.gameObject.GetComponent<BTTasks>();
            hoomanBT.GotHit(damage);            
        }        
    }
}
