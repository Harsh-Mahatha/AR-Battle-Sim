using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    private float targetFillAmount = 1f;
    private float fillSpeed = 7f;
    
    private void Start()
    {
        // Auto-find the health fill image if not assigned
        if (healthFillImage == null)
        {
            healthFillImage = GetComponentInChildren<Image>();
            if (healthFillImage == null)
            {
                Debug.LogError("HealthBar: healthFillImage not found! Make sure the Image component is a child of this object.");
                return;
            }
        }
        // Initialize fillAmount to match targetFillAmount
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = targetFillAmount;
        }
    }
    
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0)
        {
            Debug.LogWarning("HealthBar: maxHealth is 0 or negative!");
            return;
        }
        targetFillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }

    private void Update()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = Mathf.Lerp(healthFillImage.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
        }
    }
}

