using UnityEngine;
using TMPro;
using Photon.Pun;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject connectingPanel, lobbyPanel;
    public TextMeshProUGUI statusText;
    public void OnConnectPressed()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("PlayerName");
            PhotonNetwork.ConnectUsingSettings();
            connectingPanel.SetActive(true);
            Debug.Log("Player name set to: " + PhotonNetwork.LocalPlayer.NickName);
        }
    }
    void Update()
    {
        statusText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is Connected to the Servers.");
        lobbyPanel.SetActive(true);
    }

    #endregion
}
