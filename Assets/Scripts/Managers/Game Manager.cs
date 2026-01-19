using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using UnityEngine.Rendering;

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

    public void GameOver()
    {
        infoPanel.SetActive(true);
        infoText.text = "Game Over!\nReturning to Lobby...☠️☠️";
        StartCoroutine(DisableAfterSeconds(infoPanel, 3f));
        // Close the room so no new players can join and notify others
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        StartCoroutine(ReturnToLobbyAfterSeconds(3f));
    }

    public void GameWon()
    {
        infoPanel.SetActive(true);
        infoText.text = "You Won!\nReturning to Lobby...🏆🏆";
        StartCoroutine(DisableAfterSeconds(infoPanel, 3f));
        StartCoroutine(ReturnToLobbyAfterSeconds(3f));
    }

    private IEnumerator ReturnToLobbyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PhotonNetwork.LeaveRoom();
        SceneLoader.Instance.LoadScene("Level");
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
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log(PhotonNetwork.NickName + " Joined to " + PhotonNetwork.CurrentRoom.Name);
            infoText.text = "Created and joined " + PhotonNetwork.CurrentRoom.Name + " waiting for other player...";
            SpawnManager spawnManager = FindAnyObjectByType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.SpawnPlayerAt(0);
            }
        }
        else
        {
            Debug.Log("Joined to " + PhotonNetwork.CurrentRoom.Name);
            infoText.text = " You Joined " + PhotonNetwork.CurrentRoom.Name;
            SpawnManager spawnManager = FindAnyObjectByType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.SpawnPlayerAt(1);
            }
            
            StartCoroutine(AssignEnemiesWhenReady());
            StartCoroutine(DisableAfterSeconds(infoPanel, 2f));
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name);
        infoText.text = "Found Match " + newPlayer.NickName + " joined the room.";
        StartCoroutine(AssignEnemiesWhenReady());
        StartCoroutine(DisableAfterSeconds(infoPanel, 2f));
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " left " + (PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "room"));
        // If a player left while we're still in the room, we consider it a win for the remaining player
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            GameWon();
        }
    }

    private IEnumerator AssignEnemiesWhenReady()
    {
        // Wait until there are 2 PlayerMovement instances in the scene (on this client)
        yield return new WaitUntil(() => FindObjectsOfType<PlayerMovement>().Length >= 2);
        yield return null;
        
        foreach (var pm in FindObjectsOfType<PlayerMovement>())
        {
            pm.AssignEnemy();   
        }
        
        // Swap HP bars for the second player if needed
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // Find the local player's PlayerSetup and check if bars need swapping
            PlayerSetup[] playerSetups = FindObjectsOfType<PlayerSetup>();
            foreach (var setup in playerSetups)
            {
                if (setup.photonView != null && setup.photonView.IsMine)
                {
                    setup.CheckAndSwapBars();
                    break;
                }
            }
        }
    }
    #endregion
}
