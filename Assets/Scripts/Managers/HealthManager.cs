using UnityEngine;
using Photon.Pun;

public class HealthManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static HealthManager Instance;
    public static HealthManager RemoteHealthManager;

    private GameManager gameManager;
    
    // Use inherited PhotonView from MonoBehaviourPun

    [SerializeField]
    private int currentHealth = 100;
    public int maxHealth = 100;

    public Animator playerAnim;
    public GameObject healthBar;
    public GameObject enemyHealthBar;
    
    private int remoteHealth = 100;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            RemoteHealthManager = this;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        remoteHealth = maxHealth;
        // Use modern Find API when available
    #if UNITY_2023_2_OR_NEWER
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
    #else
        gameManager = FindObjectOfType<GameManager>();
    #endif
    }

    // Only the owner of this HealthManager takes damage
    public void TakeDamage(int amount)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPCTakeDamage", RpcTarget.All, amount);
        }
    }

    [PunRPC]
    private void RPCTakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (playerAnim != null)
            {
                playerAnim.SetTrigger("isDead");
            }

            // Only the owner of this HealthManager should trigger the GameOver flow (disconnect and leave)
            if (photonView.IsMine && gameManager != null)
            {
                // Close the room to prevent joins and then leave
                if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                }
                gameManager.GameOver();
                // Notify other clients that this player died so they can trigger a win locally
                try
                {
                    photonView.RPC("RPCNotifyOpponentWon", RpcTarget.Others);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Failed to RPC notify opponent of win: " + e.Message);
                }
            }
        }
        else
        {
            if (playerAnim != null)
            {
                playerAnim.SetTrigger("Damaged");
            }
        }
        
        // Update local player's own health bar
        if (photonView.IsMine && healthBar != null)
        {
            HealthBar healthBarComponent = healthBar.GetComponent<HealthBar>();
            if (healthBarComponent != null)
            {
                healthBarComponent.UpdateHealth(currentHealth, maxHealth);
            }
        }
        
        // Update remote player's enemy health bar (on the local player's UI)
        if (!photonView.IsMine && Instance != null && Instance.enemyHealthBar != null)
        {
            HealthBar enemyHealthBarComponent = Instance.enemyHealthBar.GetComponent<HealthBar>();
            if (enemyHealthBarComponent != null)
            {
                enemyHealthBarComponent.UpdateHealth(currentHealth, maxHealth);
            }
        }
    }

    [PunRPC]
    public void RPCSpawnHitEffect(Vector3 position)
    {
        // Spawn a hit VFX locally when this player is hit (so the damaged player sees it)
        GameObject impactPrefab = Resources.Load<GameObject>("SuperImpact");
        if (impactPrefab != null)
        {
            GameObject fx = Instantiate(impactPrefab, position, Quaternion.identity);
            Destroy(fx, 1.0f);
        }
        else
        {
            Debug.LogWarning("RPCSpawnHitEffect: SuperImpact prefab not found in Resources.");
        }
        // Also stop any local Super instances at this hit position so the laser doesn't visually pass through
        Super[] supers = null;
    #if UNITY_2023_2_OR_NEWER
        supers = GameObject.FindObjectsByType<Super>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    #else
        supers = FindObjectsOfType<Super>(true);
    #endif
        foreach (var s in supers)
        {
            // Stop any Super that is still active
            s.StopLocally(position);
        }
    }

    [PunRPC]
    private void RPCNotifyOpponentWon()
    {
        if (gameManager == null)
        {
#if UNITY_2023_2_OR_NEWER
            gameManager = GameObject.FindFirstObjectByType<GameManager>();
#else
            gameManager = FindObjectOfType<GameManager>();
#endif
        }
        if (gameManager != null)
        {
            gameManager.GameWon();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send local player's health to others
            stream.SendNext(currentHealth);
        }
        else
        {
            try
            {
                // Receive remote player's health
                remoteHealth = (int)stream.ReceiveNext();
                
                // Update enemy health bar on the local player's HealthManager instance
                if (Instance != null && Instance != this && Instance.enemyHealthBar != null)
                {
                    HealthBar healthBarComponent = Instance.enemyHealthBar.GetComponent<HealthBar>();
                    if (healthBarComponent != null)
                    {
                        healthBarComponent.UpdateHealth(remoteHealth, maxHealth);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error in OnPhotonSerializeView READING: " + e.Message);
            }
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetRemoteHealth()
    {
        return remoteHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
