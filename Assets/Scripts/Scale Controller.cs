using UnityEngine;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

public class ScaleController : MonoBehaviour
{
    XROrigin aRSessionOrigin;
    public Slider scaleSlider;

    void Start()
    {
        aRSessionOrigin = GetComponent<XROrigin>();
        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.AddListener(SetScale);
        }
    }

    public void SetScale(float value)
    {
        if (aRSessionOrigin != null)
        {
            aRSessionOrigin.transform.localScale = Vector3.one / value;
        }
    }
}
