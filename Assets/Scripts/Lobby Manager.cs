using UnityEngine;
using TMPro;
using Photon.Pun;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager Instance;
    public GameObject connectingPanel, lobbyPanel;
    public TextMeshProUGUI statusText;
    bool isConnecting = true;

    private void Awake()
    {
        if (Instance == null) //Singleton pattern to ensure only one instance exists
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
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
        if(isConnecting)
        statusText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is Connected to the Servers.");
        isConnecting = false;
        lobbyPanel.SetActive(true);
    }

    #endregion
}

