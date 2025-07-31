using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject newPanel;
    public GameObject existingPanel;
    public TMP_InputField nameInputField;
    public TextMeshProUGUI welcomeText;
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
    void Start()
    {
        if(PlayerPrefs.HasKey("PlayerName"))
        {
            ShowExistingPanel();
        }
        else
        {
            ShowNewPanel();
        }
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
        welcomeText.text = "Welcome " + PlayerPrefs.GetString("PlayerName") + "!";
        if (existingPanel != null)
        {
            existingPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Enter Name Panel is not assigned in the UIManager.");
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
        SceneLoader.Instance.LoadScene("Level");
        Debug.Log("Loading Battle Scene");
    }
    
    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
