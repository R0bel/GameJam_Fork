using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerDetection : MonoBehaviour
{
    private BTTasks hoomanBT;
    [SerializeField]
    private int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hooman")
        {
            hoomanBT = other.gameObject.GetComponent<BTTasks>();
            hoomanBT.GotHit(damage);
        }        
    }
}
