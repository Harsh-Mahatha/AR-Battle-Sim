using UnityEngine;

public class Punch : MonoBehaviour
{
    public float speed = 20f;
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Punch hit: " + other.name);
        Destroy(gameObject); // Destroy the punch on hit
    }
}
