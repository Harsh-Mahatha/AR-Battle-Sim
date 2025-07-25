using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;

public class Punch : MonoBehaviour
{
    public float speed = 2f;

    public int damage = 20;
    public float punchLifetime = 1f;
    public GameObject punchImpactPrefab;

    void Start()
    {
       Invoke(nameof(DestroySelf), punchLifetime);
        punchImpactPrefab = Resources.Load<GameObject>("PunchImpact");
        if (punchImpactPrefab == null)
        {
            Debug.LogError("Punch impact prefab not found in Resources folder.");
            return;
        }
    }
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Punch hit: " + other.name);
            DoDamage(damage);
            PlayHitEffect(transform.position);
            Destroy(gameObject);
        }
    }
    void DoDamage(int damageAmount)
    {
        HealthManager healthManager = FindFirstObjectByType<HealthManager>();
        if (healthManager != null)
        {
            healthManager.DealDamage(damageAmount);
        }
        else
        {
            Debug.LogWarning("HealthManager not found in the scene.");
        }
    }
    void DestroySelf()
    {
        PlayHitEffect(transform.position);
        Destroy(gameObject);
    }
    void PlayHitEffect(Vector3 position)
    {
        if (punchImpactPrefab != null)
        {
            GameObject Effect = Instantiate(punchImpactPrefab, position, Quaternion.identity);
            Destroy(Effect, 1f); 
        }
    }
}
