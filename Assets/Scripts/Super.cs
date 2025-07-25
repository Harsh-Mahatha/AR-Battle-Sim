using UnityEngine;

public class Super : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float speed = 40f;
    public float maxLength = 50f;
    public int damage = 40; 
    public GameObject impactVFX;
    public GameObject startVFX;
    public LayerMask hitMask;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private float currentLength = 0f;
    private bool hitDetected = false;
    bool isEndVFXSpawned = false, isStartVFXSpawned = false;
    GameObject endSpawnedEffect, startSpawnedEffect;

    void Start()
    {
        startPoint = transform.position;
        impactVFX = Resources.Load<GameObject>("SuperImpact");
        startVFX = Resources.Load<GameObject>("SuperStartEffect");

        RaycastHit hit;
        if (Physics.Raycast(startPoint, transform.forward, out hit, maxLength, hitMask))
        {
            endPoint = hit.point;
            hitDetected = true;
        }
        else
        {
            endPoint = startPoint + transform.forward * maxLength;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, startPoint);
    }

    void Update()
    {
        // Move the end point of the line forward
        if (currentLength < Vector3.Distance(startPoint, endPoint))
        {
            if (!isStartVFXSpawned && startVFX != null)
            {
                startSpawnedEffect = Instantiate(startVFX, startPoint, Quaternion.identity);
                isStartVFXSpawned = true;
            }
            currentLength += speed * Time.deltaTime;
            Vector3 currentEnd = Vector3.Lerp(startPoint, endPoint, currentLength / Vector3.Distance(startPoint, endPoint));
            lineRenderer.SetPosition(1, currentEnd);
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(endPoint, 0.5f, hitMask);
            bool enemyHit = false;
            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    enemyHit = true;
                    break;
                }
            }

            if (hitDetected && impactVFX != null && !isEndVFXSpawned && enemyHit)
            {
                endSpawnedEffect = Instantiate(impactVFX, endPoint, Quaternion.identity);
                isEndVFXSpawned = true;
                DoDamage();
            }

            Destroy(gameObject, 0.7f); // Clean up laser after impact
            Destroy(endSpawnedEffect, 0.8f); // Clean up impact effect after a short delay
            Destroy(startSpawnedEffect, 0.8f); // Clean up start effect after a short delay
        }
    }

    void DoDamage()
    {
        HealthManager healthManager = FindFirstObjectByType<HealthManager>();
        if (healthManager != null)
        {
            healthManager.DealDamage(damage);
            Debug.Log("Super attack dealt " + damage + " damage.");
        }
        else
        {
            Debug.LogWarning("HealthManager not found in the scene.");
        }
    }
}
