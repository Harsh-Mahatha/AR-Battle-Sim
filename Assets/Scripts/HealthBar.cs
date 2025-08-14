using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    private float targetFillAmount = 1f;
    private float fillSpeed = 7f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; 
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        targetFillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }

    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = Mathf.Lerp(healthFillImage.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
        }
    }
}
