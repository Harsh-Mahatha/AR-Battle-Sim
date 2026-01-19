using UnityEngine;
using Photon.Pun;

public class Super : MonoBehaviourPunCallbacks
{
    public LineRenderer lineRenderer;
    public float speed = 40f;
    public float maxLength = 50f;
    public int damage = 40; 
    public GameObject impactVFX;
    public GameObject startVFX;
    public LayerMask hitMask;
    [Header("Debug")]
    public bool showDebugGizmos = false;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private float currentLength = 0f;
    private bool hitDetected = false;
    private bool damageDealt = false;
    private bool hasStopped = false;
    bool isEndVFXSpawned = false, isStartVFXSpawned = false;
    GameObject endSpawnedEffect, startSpawnedEffect;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
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
        // Spawn start effect once
        if (!isStartVFXSpawned && startVFX != null)
        {
            startSpawnedEffect = Instantiate(startVFX, startPoint, Quaternion.identity);
            isStartVFXSpawned = true;
        }

        // Only the owner of this Super should run extension / hit detection
        if (photonView != null && !photonView.IsMine)
        {
            return;
        }

        // If we've already hit and stopped, exit early
        if (hasStopped)
        {
            return;
        }

        // Move the end point of the line forward
        if (currentLength < Vector3.Distance(startPoint, endPoint))
        {
            float previousLength = currentLength;
            currentLength += speed * Time.deltaTime;
            Vector3 currentEnd = Vector3.Lerp(startPoint, endPoint, currentLength / Vector3.Distance(startPoint, endPoint));
            
            // Check for hits continuously as the laser extends using OverlapSphere
            // This is more reliable than raycast and works like Punch's OnTriggerEnter
            Vector3 previousEnd = Vector3.Lerp(startPoint, endPoint, previousLength / Vector3.Distance(startPoint, endPoint));
            Vector3 checkPosition = Vector3.Lerp(previousEnd, currentEnd, 0.5f); // Check at midpoint
            float checkRadius = 0.5f; // Radius to check for enemy colliders
            
            // Use OverlapSphere to detect enemies (doesn't require LayerMask, checks all colliders)
            Collider[] hitColliders = Physics.OverlapSphere(checkPosition, checkRadius);
            
            // Debug visualization
            if (showDebugGizmos)
            {
                Debug.DrawLine(previousEnd, currentEnd, Color.red, 0.1f);
                Debug.DrawRay(checkPosition, Vector3.up * checkRadius, Color.yellow, 0.1f);
                if (hitColliders.Length > 0)
                {
                    Debug.Log($"Super checking {hitColliders.Length} colliders at {checkPosition}");
                }
            }
            
            foreach (var collider in hitColliders)
            {
                // Check if we hit an enemy by tag (more reliable than layer mask)
                if (collider.CompareTag("Enemy"))
                {
                    // Verify it's actually an enemy player by checking PhotonView
                    PhotonView enemyPhotonView = collider.GetComponent<PhotonView>();
                    if (enemyPhotonView == null)
                    {
                        enemyPhotonView = collider.GetComponentInParent<PhotonView>();
                    }
                    
                    if (enemyPhotonView != null)
                    {
                        // Verify we're hitting someone else (not ourselves)
                        // Check if the enemy's owner is different from local player
                        if (enemyPhotonView.Owner != PhotonNetwork.LocalPlayer)
                        {
                            // Stop the laser at the hit point
                            Vector3 hitPoint = collider.ClosestPoint(checkPosition);
                            currentEnd = hitPoint;
                            endPoint = hitPoint;
                            currentLength = Vector3.Distance(startPoint, endPoint);
                            hasStopped = true;
                            
                            // Play hit effect immediately
                            if (impactVFX != null && !isEndVFXSpawned)
                            {
                                Vector3 hitDirection = (endPoint - startPoint).normalized;
                                endSpawnedEffect = Instantiate(impactVFX, hitPoint, Quaternion.LookRotation(-hitDirection));
                                isEndVFXSpawned = true;
                                Debug.Log("Super hit enemy: " + collider.name + " at position: " + hitPoint);
                            }
                            // Tell all clients to stop the laser at this position (keeps visuals in sync)
                            if (photonView != null)
                            {
                                photonView.RPC("RPCStopLaser", RpcTarget.All, hitPoint);
                            }
                            
                            // Deal damage once
                            if (!damageDealt)
                            {
                                DoDamage(collider, enemyPhotonView, hitPoint);
                                damageDealt = true;
                            }
                            break; // Exit loop once we hit an enemy
                        }
                    }
                }
            }
            
            lineRenderer.SetPosition(1, currentEnd);
        }
        else
        {
            // Laser has reached its end point
            hasStopped = true;
            
            // Final check for enemy at end point (in case we missed during extension)
            if (!damageDealt)
            {
                Collider[] hitColliders = Physics.OverlapSphere(endPoint, 0.5f);
                foreach (var collider in hitColliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        PhotonView enemyPhotonView = collider.GetComponent<PhotonView>();
                        if (enemyPhotonView == null)
                        {
                            enemyPhotonView = collider.GetComponentInParent<PhotonView>();
                        }
                        
                        if (enemyPhotonView != null)
                        {
                            // Verify we're hitting someone else (not ourselves)
                            if (enemyPhotonView.Owner != PhotonNetwork.LocalPlayer)
                            {
                                // Play hit effect
                                if (impactVFX != null && !isEndVFXSpawned)
                                {
                                    Vector3 hitDirection = (endPoint - startPoint).normalized;
                                    endSpawnedEffect = Instantiate(impactVFX, endPoint, Quaternion.LookRotation(-hitDirection));
                                    isEndVFXSpawned = true;
                                    Debug.Log("Super hit enemy at end point: " + collider.name);
                                }
                                
                                // Tell all clients to stop the laser at this position
                                if (photonView != null)
                                {
                                    photonView.RPC("RPCStopLaser", RpcTarget.All, endPoint);
                                }

                                // Deal damage
                                DoDamage(collider, enemyPhotonView, endPoint);
                                damageDealt = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            // Clean up after a delay
            Destroy(gameObject, 0.7f);
            if (endSpawnedEffect != null)
            {
                Destroy(endSpawnedEffect, 0.8f);
            }
            if (startSpawnedEffect != null)
            {
                Destroy(startSpawnedEffect, 0.8f);
            }
        }
    }

    void DoDamage(Collider enemyCollider, PhotonView enemyPhotonView, Vector3 hitPosition)
    {
        if (enemyCollider == null || enemyPhotonView == null)
        {
            Debug.LogWarning("DoDamage called with null collider or PhotonView!");
            return;
        }
        
        // Only the owner of the Super can deal damage (prevent remote execution)
        if (photonView != null && !photonView.IsMine)
        {
            Debug.LogWarning("Super attack not owned by local player! Ignoring damage.");
            return;
        }
        
        // Double-check we're not hitting ourselves
        if (enemyPhotonView.Owner == PhotonNetwork.LocalPlayer)
        {
            Debug.LogWarning("Attempted to damage own player! Skipping damage.");
            return;
        }
        
        Debug.Log("DoDamage called for: " + enemyCollider.name + " | Owner: " + enemyPhotonView.Owner.NickName);
        
        HealthManager healthManager = enemyCollider.GetComponentInParent<HealthManager>();
        if (healthManager != null && healthManager.photonView != null)
        {
            // Verify the HealthManager belongs to the enemy (not us)
            if (healthManager.photonView.Owner != PhotonNetwork.LocalPlayer)
            {
            // Request the enemy client to spawn the hit VFX locally at the provided hit position
            healthManager.photonView.RPC("RPCSpawnHitEffect", enemyPhotonView.Owner, hitPosition);

            // Send damage RPC only to the hit player's owner so only they apply the damage locally
            healthManager.photonView.RPC("RPCTakeDamage", enemyPhotonView.Owner, damage);
                Debug.Log("Super attack dealt " + damage + " damage to " + enemyCollider.name + " (Owner: " + healthManager.photonView.Owner.NickName + ")");
                
                // Destroy line renderer and effects only after successfully dealing damage
                if (lineRenderer != null)
                {
                    Destroy(lineRenderer.gameObject, 0.8f);
                }
                if (endSpawnedEffect != null)
                {
                    Destroy(endSpawnedEffect, 0.8f);
                }
                if (startSpawnedEffect != null)
                {
                    Destroy(startSpawnedEffect, 0.8f);
                }
            }
            else
            {
                Debug.LogWarning("HealthManager belongs to local player! Skipping damage to prevent self-damage.");
            }
        }
        else
        {
            Debug.LogWarning("HealthManager not found on enemy for Super attack. Collider: " + enemyCollider.name);
        }
    }

    [PunRPC]
    void RPCStopLaser(Vector3 hitPos)
    {
        // Ensure line visually ends at hitPos on all clients
        endPoint = hitPos;
        hasStopped = true;
        currentLength = Vector3.Distance(startPoint, endPoint);
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(1, endPoint);
        }

        // Spawn impact VFX on clients that haven't spawned it yet
        if (!isEndVFXSpawned && impactVFX != null)
        {
            Vector3 hitDirection = (endPoint - startPoint).normalized;
            endSpawnedEffect = Instantiate(impactVFX, endPoint, Quaternion.LookRotation(-hitDirection));
            isEndVFXSpawned = true;
        }

        // Schedule cleanup of effects and this object
        if (endSpawnedEffect != null)
        {
            Destroy(endSpawnedEffect, 0.8f);
        }
        if (startSpawnedEffect != null)
        {
            Destroy(startSpawnedEffect, 0.8f);
        }
        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject, 0.8f);
        }
        // Also destroy the GO on non-owner clients after a small delay to match owner behavior
        if (photonView != null && !photonView.IsMine)
        {
            Destroy(gameObject, 0.7f);
        }
    }

    // Called locally to stop this Super's visuals at a specific world position
    public void StopLocally(Vector3 hitPos)
    {
        endPoint = hitPos;
        hasStopped = true;
        currentLength = Vector3.Distance(startPoint, endPoint);
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(1, endPoint);
        }

        if (!isEndVFXSpawned && impactVFX != null)
        {
            Vector3 hitDirection = (endPoint - startPoint).normalized;
            endSpawnedEffect = Instantiate(impactVFX, endPoint, Quaternion.LookRotation(-hitDirection));
            isEndVFXSpawned = true;
        }

        // cleanup visuals
        if (endSpawnedEffect != null)
        {
            Destroy(endSpawnedEffect, 0.8f);
        }
        if (startSpawnedEffect != null)
        {
            Destroy(startSpawnedEffect, 0.8f);
        }
        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject, 0.8f);
        }

        // destroy the GameObject locally after a short delay
        Destroy(gameObject, 0.7f);
    }
}
