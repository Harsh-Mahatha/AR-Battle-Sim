using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;


public class SpawnManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public Transform spawnPoint;
    void Update()
    {

    }

    #region  Photon
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, quaternion.identity);  
        }   
    }

    #endregion
}
