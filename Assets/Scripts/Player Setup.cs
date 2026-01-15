using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI playerName, enemyName;
    public GameObject playerHeathBar, enemyHealthBar;

    void Start()
    {
        if (photonView.IsMine)
        {
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<Attacks>().enabled = true;
            transform.Find("Player Canvas").gameObject.SetActive(true);

            // Find this player's own HealthManager
            HealthManager healthManager = GetComponent<HealthManager>();
            if (healthManager == null)
            {
                healthManager = GetComponentInParent<HealthManager>();
            }
            
            if (healthManager != null)
            {
                // Set player's own animator and health bars
                healthManager.playerAnim = GetComponent<Animator>();
                healthManager.healthBar = GameObject.FindWithTag("PlayerHP");
                healthManager.enemyHealthBar = GameObject.FindWithTag("EnemyHP");
            }
            else
            {
                Debug.LogError("HealthManager not found on player!");
            }
            SetPlayerName("You", Color.blue);
        }
        else
        {
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<Attacks>().enabled = false;
            transform.Find("Player Canvas").gameObject.SetActive(false);
            GetComponentInChildren<Renderer>().material.color = Color.red;
        }
    }

    void SetPlayerName(string name, Color color)
    {
        playerName.text = name;
        playerName.color = color;
    }

    public void SwapHPBars()
    {
        Debug.Log("Swapping HP Bars for second player");
        if (!photonView.IsMine) return;
        
        // Get health bars from HealthManager if fields aren't assigned
        GameObject playerBar = playerHeathBar;
        GameObject enemyBar = enemyHealthBar;
        
        if (playerBar == null || enemyBar == null)
        {
            HealthManager healthManager = GetComponent<HealthManager>();
            if (healthManager == null)
            {
                healthManager = GetComponentInParent<HealthManager>();
            }
            
            if (healthManager != null)
            {
                if (playerBar == null) playerBar = healthManager.healthBar;
                if (enemyBar == null) enemyBar = healthManager.enemyHealthBar;
            }
        }
        
        // Get RectTransform components for UI elements
        RectTransform playerRect = playerBar?.GetComponent<RectTransform>();
        RectTransform enemyRect = enemyBar?.GetComponent<RectTransform>();
        
        if (playerRect != null && enemyRect != null)
        {
            // Swap the anchored positions (x coordinates)
            Vector2 playerPos = playerRect.anchoredPosition;
            Vector2 enemyPos = enemyRect.anchoredPosition;
            
            // Swap x positions (keep y the same)
            playerRect.anchoredPosition = new Vector2(enemyPos.x, playerPos.y);
            enemyRect.anchoredPosition = new Vector2(playerPos.x, enemyPos.y);
            
            Debug.Log($"Swapped HP bars - Player: {playerRect.anchoredPosition.x}, Enemy: {enemyRect.anchoredPosition.x}");
        }
        else
        {
            Debug.LogError("Could not find RectTransform components on health bars!");
        }
    }
    
    // Call this after health bars are assigned to swap if needed
    public void CheckAndSwapBars()
    {
        if (!photonView.IsMine) return;
        
        // Check if this is the second player (should swap bars)
        // When there are 2 players, the one with higher ActorNumber joined second
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // Get all actor numbers and find the max
            int maxActorNumber = 0;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.ActorNumber > maxActorNumber)
                {
                    maxActorNumber = player.ActorNumber;
                }
            }
            
            // If we have the higher ActorNumber, we're the second player and should swap
            if (PhotonNetwork.LocalPlayer.ActorNumber == maxActorNumber)
            {
                Debug.Log("Second player detected - swapping HP bars");
                SwapHPBars();
            }
        }
    }

    public void SetEnemyName(string name, Color color)
    {
        enemyName.text = name;
        enemyName.color = color;
    }
}
