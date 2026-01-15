using UnityEngine;
using Photon.Pun;

public class Punch : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed = 2f;

    public int damage = 20;
    public float punchLifetime = 1f;
    public GameObject punchImpactPrefab;

    private PhotonView photonView;

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float lag;

    void Awake()
    {
        networkPosition = transform.position;
        networkRotation = transform.rotation;
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        Invoke(nameof(DestroySelf), punchLifetime);
        punchImpactPrefab = Resources.Load<GameObject>("PunchImpact");
        if (punchImpactPrefab == null)
        {
            Debug.LogError("Punch impact prefab not found in Resources folder.");
            return;
        }
    }
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * 10);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        PhotonView otherPhotonView = other.GetComponent<PhotonView>();
        if (otherPhotonView != null && otherPhotonView.CompareTag("Enemy"))
        {
            // Only process hit if we own the punch and hit someone else's object
            if (otherPhotonView.Owner != photonView.Owner)
            {
                Debug.Log("Punch hit: " + other.name);
                // Find the opponent's HealthManager and apply damage through RPC
                HealthManager healthManager = other.GetComponentInParent<HealthManager>();
                if (healthManager != null && healthManager.photonView != null)
                {
                    // Call RPC directly on the enemy's PhotonView to apply damage
                    // This ensures the enemy processes the damage on their side
                    healthManager.photonView.RPC("RPCTakeDamage", RpcTarget.All, damage);
                }
                photonView.RPC("RPCPlayHitEffect", RpcTarget.All, transform.position);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    [PunRPC]
    void RPCPlayHitEffect(Vector3 position)
    {
        if (punchImpactPrefab != null)
        {
            GameObject Effect = Instantiate(punchImpactPrefab, position, Quaternion.identity);
            Destroy(Effect, 1f);
        }
    }

    void DestroySelf()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPCPlayHitEffect", RpcTarget.All, transform.position);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Network player, receive data
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        }
    }
}
