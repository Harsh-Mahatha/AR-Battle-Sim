using UnityEngine;

public class Super : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float speed = 40f;
    public float maxLength = 50f;
    public GameObject impactVFX;
    public LayerMask hitMask;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private float currentLength = 0f;
    private bool hitDetected = false;
    bool isspawned = false;
    GameObject spwanedEffect;

    void Start()
    {
        startPoint = transform.position;
        impactVFX = Resources.Load<GameObject>("SuperImpact");

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
            currentLength += speed * Time.deltaTime;
            Vector3 currentEnd = Vector3.Lerp(startPoint, endPoint, currentLength / Vector3.Distance(startPoint, endPoint));
            lineRenderer.SetPosition(1, currentEnd);
        }
        else
        {

            if (hitDetected && impactVFX != null && !isspawned)
            {
                spwanedEffect = Instantiate(impactVFX, endPoint, Quaternion.identity);
                isspawned = true;
            }

            Destroy(gameObject, 0.7f); // Clean up laser after impact
            Destroy(spwanedEffect, 0.7f); // Clean up impact effect after a short delay
        }
    }
}
