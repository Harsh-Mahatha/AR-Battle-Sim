using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
public GameObject newPanel;
public GameObject existingPanel, lobbyPanel;
public TMP_InputField nameInputField;
public TextMeshProUGUI welcomeText;
private void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

void Start()
{
    UpdatePanelsOnSceneLoad();
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    UpdatePanelsOnSceneLoad();
}

private void UpdatePanelsOnSceneLoad()
{
    HideAllPanels();

    if (Photon.Pun.PhotonNetwork.IsConnectedAndReady)
    {
        ShowLobbyPanel();
        return;
    }

    if (PlayerPrefs.HasKey("PlayerName"))
    {
        ShowExistingPanel();
    }
    else
    {
        ShowNewPanel();
    }
}

private void HideAllPanels()
{
    if (newPanel != null) newPanel.SetActive(false);
    if (existingPanel != null) existingPanel.SetActive(false);
    if (lobbyPanel != null) lobbyPanel.SetActive(false);
}

public void ShowNewPanel()
{
    if (newPanel != null)
    {
        newPanel.SetActive(true);
    }
    else
    {
        Debug.LogWarning("Enter Name Panel is not assigned in the UIManager.");
    }
}

 public void ShowExistingPanel()
{
    if (welcomeText != null)
    {
        welcomeText.text = "Welcome " + PlayerPrefs.GetString("PlayerName") + "!";
    }

    if (existingPanel != null)
    {
        existingPanel.SetActive(true);
    }
    else
    {
        Debug.LogWarning("Enter Name Panel is not assigned in the UIManager.");
    }
}

public void ShowLobbyPanel()
{
    if (lobbyPanel != null)
    {
        lobbyPanel.SetActive(true);
    }
    else
    {
        Debug.LogWarning("Lobby Panel is not assigned in the UIManager.");
    }
}

public void SavePlayerName()
{
    string playerName = nameInputField.text.Trim();

    if (!string.IsNullOrEmpty(playerName))
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        Debug.Log("Name saved: " + playerName);

        ShowExistingPanel();
    }
    else
    {
        Debug.Log("Please enter a valid name.");
    }
}

public void LoadBattle()
{
    if (SceneLoader.Instance != null)
    {
        SceneLoader.Instance.LoadScene("Level");
        Debug.Log("Loading Battle Scene");
    }
    else
    {
        Debug.LogError("SceneLoader.Instance is null!");
    }
}

public void QuitGame()
{
    Debug.Log("Game Quit");
    Application.Quit();
}
}
