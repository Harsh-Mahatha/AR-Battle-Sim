using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject infoPanel, findMatchButton;
    public TextMeshProUGUI infoText;
    public GameObject controlsHud;
    void Start()
    {
        infoPanel.SetActive(true);
        //infoText.text = "Click Find Match Button to Play";
    }
    public void OnFindMatchClicked()
    {
        PhotonNetwork.JoinRandomRoom();
        infoText.text = "Finding Matches...";
        findMatchButton.SetActive(false);
    }

    private IEnumerator DisableAfterSeconds(GameObject gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
        controlsHud.SetActive(true);
    }



    #region Photon
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateNewRoom();
        infoText.text = message;
    }

    void CreateNewRoom()
    {
        string roomName = "Room" + Random.Range(1, 100);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {   if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log(PhotonNetwork.NickName + "Joined to " + PhotonNetwork.CurrentRoom.Name);
            infoText.text = "Created and joined " + PhotonNetwork.CurrentRoom.Name + " waiting for other player...";
            FindAnyObjectByType<SpawnManager>().SpawnPlayerAt(0);
        }
        else
        {
            Debug.Log("Joined to " + PhotonNetwork.CurrentRoom.Name);
            infoText.text = " You Joined " + PhotonNetwork.CurrentRoom.Name;
            FindAnyObjectByType<SpawnManager>().SpawnPlayerAt(1);
            StartCoroutine(DisableAfterSeconds(infoPanel, 2f));
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "Joined to " + PhotonNetwork.CurrentRoom.Name);
        infoText.text = "Found Match " + newPlayer.NickName + " joined the room.";
        StartCoroutine(DisableAfterSeconds(infoPanel, 2f));
    }

    #endregion

}
