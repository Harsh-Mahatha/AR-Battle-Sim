using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class AdjustmentandPlacementController : MonoBehaviour
{
    public GameObject placeButton, adjustButton, findMatchButton, scaleSlider;
    public TextMeshProUGUI infoText;

    ARPlacementManager arPlacementManager;
    ARPlaneManager arPlaneManager;

    void Awake()
    {
        arPlacementManager = GetComponent<ARPlacementManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
    }
    void Start()
    {
        placeButton.SetActive(true);
        scaleSlider.SetActive(true);
        adjustButton.SetActive(false);
        findMatchButton.SetActive(false);
        infoText.text = "Move the Phone to place arena. Adjust the placement if needed.";
    }
    public void OnPlaceButtonClicked()
    {
        placeButton.SetActive(false);
        adjustButton.SetActive(true);
        findMatchButton.SetActive(true);
        scaleSlider.SetActive(false);
        arPlacementManager.enabled = false; // Disable placement
        arPlaneManager.enabled = false; // Disable plane detection
        SetPlanes(false); // Hide planes
        infoText.text = "Arena placed. Find a match or adjust placement.";
    }

    public void OnAdjustButtonClicked()
    {
        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        scaleSlider.SetActive(true);
        findMatchButton.SetActive(false);
        arPlacementManager.enabled = true;
        arPlaneManager.enabled = true; // Enable plane detection
        SetPlanes(true); // Show planes
        infoText.text = "Adjust the placement of the arena.";
    }

    void SetPlanes(bool status)
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(status);
        }
    }    
}
