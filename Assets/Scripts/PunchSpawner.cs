using UnityEngine;

public class PunchSpawner : MonoBehaviour
{
    public GameObject punchPrefab;
    public Transform spawnPoint;

    public void SpawnPunch()
    {
        if (punchPrefab == null)
        {
            punchPrefab = Resources.Load<GameObject>("Punch");
            if (punchPrefab == null)
            {
                Debug.LogError("Punch prefab not found in Resources folder.");
                return;
            }
        }

        GameObject spawnedPunch = Instantiate(punchPrefab, spawnPoint.position, transform.rotation);
    }
}
