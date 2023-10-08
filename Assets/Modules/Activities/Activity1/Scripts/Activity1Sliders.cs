using UnityEngine;

public class Activity1Sliders : MonoBehaviour
{
    [SerializeField] private CustomSlider thetaSlider;
    [SerializeField] private CustomSlider phiSlider;
    [SerializeField] private CustomSlider psiSlider;
    [SerializeField] private CustomSlider zoomSlider;

    private float initialZoomValue = 1;

    public void Reset()
    {
        if (thetaSlider) thetaSlider.value = 0;
        if (phiSlider) phiSlider.value = 0;
        if (psiSlider) psiSlider.value = 0;
        if (zoomSlider) zoomSlider.value = initialZoomValue;
    }

    public void SetInteractable(bool value)
    {
        if (thetaSlider) thetaSlider.interactable = value;
        if (phiSlider) phiSlider.interactable = value;
        if (psiSlider) psiSlider.interactable = value;
        if (zoomSlider) zoomSlider.interactable = value;
    }

    public void SaveInitialZoomValue()
    {
        if (zoomSlider) initialZoomValue = zoomSlider.value;
    }
}
