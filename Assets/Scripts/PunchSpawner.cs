using UnityEngine;
using Photon.Pun;

public class PunchSpawner : MonoBehaviourPunCallbacks
{
    public GameObject punchPrefab;
    public Transform spawnPoint;
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void SpawnPunch()
    {
        if (!photonView.IsMine) return;

        if (punchPrefab == null)
        {
            punchPrefab = Resources.Load<GameObject>("Punch");
            if (punchPrefab == null)
            {
                Debug.LogError("Punch prefab not found in Resources folder.");
                return;
            }
        }

        // Use PhotonNetwork.Instantiate instead of GameObject.Instantiate
        PhotonNetwork.Instantiate(punchPrefab.name, spawnPoint.position, transform.rotation);
    }
}
