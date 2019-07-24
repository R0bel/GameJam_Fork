using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhotonControl : MonoBehaviour, IPunInstantiateMagicCallback
{
    private GameManager gameManager;
    private ARLevel currentLevel;

    private void OnEnable()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        //PhotonView photonView = PhotonView.Get(this);
        // photonView.RPC("UpdatePlayerPosition", RpcTarget.All, transform.localPosition);
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

    /*
    [PunRPC]
    void UpdatePlayerPosition(Vector3 _position)
    {
        transform.localPosition = _position;
    }
    */

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            /*
            stream.SendNext(ship.transform.position);
            stream.SendNext(ship.transform.rotation);
            stream.SendNext(Rb.velocity);
            */

        }
        else
        {
            /*
            _netPos = (Vector3)stream.ReceiveNext();
            _netRot = (Quaternion)stream.ReceiveNext();
            Rb.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.time - info.timestamp));
            print(lag);
            _netPos += (Rb.velocity * lag);
            if (Vector3.Distance(ship.transform.position, _netPos) > 20.0f) // more or less a replacement for CheckExitScreen function on remote clients
            {
                ship.transform.position = _netPos;
            }
            */
        }
    }
}
