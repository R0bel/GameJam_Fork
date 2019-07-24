using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhotonControl : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
{
    private GameManager gameManager;
    private ARLevel currentLevel;
    private Character character;

    private Vector3 truePosition;
    private Quaternion trueRotation;
    private Vector3 trueSpeed;

    private int positionCheckCounter = 0;
    [SerializeField]
    private int positionCheckRate = 100;

    private void OnEnable()
    {
        gameManager = GameManager.Instance;
        character = GetComponent<Character>();
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
                // transform.localScale = new Vector3(1f, 1f, 1f);

                gameManager.Events.OnCharacterSpawned(this.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        gameManager.Events.OnCharacterDespawned(this.gameObject);
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
        if (PhotonView.Get(this).IsMine) return;

        if (positionCheckCounter == 0)
        {
            if (!IsNaN(truePosition)) transform.localPosition = truePosition;
        }

        if (positionCheckCounter > positionCheckRate)
        {
            positionCheckCounter = 1;
            if (!IsNaN(truePosition)) transform.localPosition = truePosition;
        }
        positionCheckCounter++;
        // transform.localPosition = Vector3.Lerp(transform.localPosition, truePosition, Time.deltaTime * 5);
        if (!IsNaN(trueRotation) && !IsNaN(transform.localRotation) && character != null)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, trueRotation, Time.deltaTime * character.TurnSpeed);
        }        
    }

    private bool IsNaN(Quaternion q)
    {
        return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
    }

    private bool IsNaN(Vector3 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
    }
}
