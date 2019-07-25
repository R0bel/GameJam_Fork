using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoomanPhotonControl : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
{
    private GameManager gameManager;
    private ARLevel currentLevel;
    private BTTasks hoomanControl;
    private PhotonView view;

    private float m_Distance;
    private float m_Angle;

    private PhotonView m_PhotonView;

    private Vector3 m_Direction;
    private Vector3 m_NetworkPosition;
    private Vector3 m_StoredPosition;

    private Quaternion m_NetworkRotation;

    bool m_firstTake = false;

    public void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        m_StoredPosition = transform.localPosition;
        m_NetworkPosition = Vector3.zero;

        m_NetworkRotation = Quaternion.identity;
    }

    void OnEnable()
    {
        gameManager = GameManager.Instance;
        hoomanControl = GetComponent<BTTasks>();
        m_firstTake = true;
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
            }
        }
    }

    public void Update()
    {
        if (!this.m_PhotonView.IsMine)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            this.m_Direction = transform.localPosition - this.m_StoredPosition;
            this.m_StoredPosition = transform.localPosition;

            stream.SendNext(transform.localPosition);
            stream.SendNext(this.m_Direction);

            stream.SendNext(transform.localRotation);
        }
        else
        {
            this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
            this.m_Direction = (Vector3)stream.ReceiveNext();

            if (m_firstTake)
            {
                transform.localPosition = this.m_NetworkPosition;
                this.m_Distance = 0f;
            }
            else
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                this.m_NetworkPosition += this.m_Direction * lag;
                this.m_Distance = Vector3.Distance(transform.localPosition, this.m_NetworkPosition);
            }

            this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

            if (m_firstTake)
            {
                this.m_Angle = 0f;
                transform.localRotation = this.m_NetworkRotation;
            }
            else
            {
                this.m_Angle = Quaternion.Angle(transform.localRotation, this.m_NetworkRotation);
            }

            if (m_firstTake)
            {
                m_firstTake = false;
            }
        }
    }
}
