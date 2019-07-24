using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhotonControl : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
{
    private GameManager gameManager;
    private ARLevel currentLevel;
    [SerializeField]
    private Rigidbody rigid;

    private Vector3 truePosition;
    private Quaternion trueRotation;
    private Vector3 trueSpeed;
    private float lastRotation;
    private float trueAngularSpeed;

    private void OnEnable()
    {
        gameManager = GameManager.Instance;
    }

    void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (gameManager != null)
        {
            currentLevel = gameManager.AR.ActiveLevel;            
            if (currentLevel != null)
            {
                // scaling character
                transform.parent = currentLevel.transform;
                transform.position = currentLevel.transform.position;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }        
    }

    /// <summary>
    /// this is where data is sent and received for this Component from the PUN Network.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <param name="info">Info.</param>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonView.Get(this).IsMine)
            {
                stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
            }
        }
        else
        {
            if (!PhotonView.Get(this).IsMine)
            {
                truePosition = (Vector3)stream.ReceiveNext();
                trueRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, truePosition, Time.deltaTime * 5);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, trueRotation, Time.deltaTime * 5);
    }
}
