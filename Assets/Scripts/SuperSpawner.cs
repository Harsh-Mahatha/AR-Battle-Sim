using UnityEngine;

public class SuperSpawner : MonoBehaviour
{
    public GameObject superPrefab;
    public Transform spawnPoint;

    public void SpawnSuper()
    {
        if (superPrefab == null)
        {
            superPrefab = Resources.Load<GameObject>("Super");
            if (superPrefab == null)
            {
                Debug.LogError("Punch prefab not found in Resources folder.");
                return;
            }
        }

        Instantiate(superPrefab, spawnPoint.position, transform.rotation);
    }
}
