using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;


public class SpawnManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public Transform [] spawnPoints;

    public void SpawnPlayerAt(int posi)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[posi].position, quaternion.identity);
        }
    }
}
