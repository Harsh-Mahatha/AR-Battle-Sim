using UnityEngine;
using Photon.Pun;

public class HealthManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static HealthManager Instance;
    public static HealthManager RemoteHealthManager;
    
    public PhotonView photonView;

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
        photonView = GetComponent<PhotonView>();
        currentHealth = maxHealth;
        remoteHealth = maxHealth;
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
