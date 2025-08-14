using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;
    public int playerHealth = 100, enemyHealth = 100;

    public int maxHealth = 100;

    public Animator playerAnim, enemyAnim;

    [SerializeField] private GameObject playerHPBar, enemyHPBar;

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

    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Playerdead();
        }
        playerAnim.SetTrigger("Damaged");
        playerHPBar.GetComponent<HealthBar>().UpdateHealth(playerHealth, maxHealth);
        Debug.Log(" Player Health: " + playerHealth);
    }

    public void DealDamage(int amount)
    {
        enemyHealth -= amount;
        if (enemyHealth <= 0)
        {
            enemyHealth = 0;
            Enemydead();
        }
        enemyAnim.SetTrigger("Damaged");
        enemyHPBar.GetComponent<HealthBar>().UpdateHealth(enemyHealth, maxHealth);
        Debug.Log(" Enemy Health: " + enemyHealth);
    }
    public int GetCurrentPlayerHealth()
    {
        return playerHealth;
    }

    public int GetCurrentEnemyHealth()
    {
        return enemyHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    private void Playerdead()
    {
        Debug.Log("Player is dead");
        playerAnim.SetTrigger("isDead");
    }

     private void Enemydead()
    {
        Debug.Log("Enemy is dead");
        enemyAnim.SetTrigger("isDead");
    }
}
