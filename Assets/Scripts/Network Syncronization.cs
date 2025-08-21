using UnityEngine;
using Photon.Pun;
using System;
public class NetworkSyncronization : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;
    Vector3 networkPosition;
    Quaternion networkRotation;
    public bool synchronizeVelocity = true, synchronizeAngles = true, canTP = true;
    float distance, angle;
    public float tpDistance = 1f;

    void Start()
    {
        if (!photonView.IsMine)
        {
            Debug.Log("enemy posi assigned");
            gameObject.GetComponent<PlayerMovement>().enemyTransform = transform;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        networkPosition = new Vector3();
        networkRotation = new Quaternion();
        photonView = GetComponent<PhotonView>();
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            if (synchronizeVelocity)
            {
                stream.SendNext(rb.linearVelocity);
            }
            if (synchronizeAngles)
            {
                stream.SendNext(rb.angularVelocity);
            }
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = ((Quaternion)stream.ReceiveNext()).normalized;

            if (canTP)
            {
                if (Vector3.Distance(rb.position, networkPosition) > tpDistance)
                {
                    rb.position = networkPosition;
                }
            }

            if (synchronizeAngles || synchronizeVelocity)
            {
                float lag = MathF.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                if (synchronizeVelocity)
                {
                    rb.linearVelocity = (Vector3)stream.ReceiveNext();
                    networkPosition += rb.linearVelocity * lag;

                    distance = Vector3.Distance(rb.position, networkPosition);
                }
                if (synchronizeAngles)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();
                    networkRotation = Quaternion.Euler(rb.angularVelocity * lag) * networkRotation;

                    angle = Quaternion.Angle(rb.rotation, networkRotation);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkPosition, distance * (1f/PhotonNetwork.SerializationRate));
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRotation, angle * (1f/PhotonNetwork.SerializationRate));
        }
    }
}
