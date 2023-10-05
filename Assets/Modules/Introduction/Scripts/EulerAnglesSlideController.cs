using Slides;
using UnityEngine;
using UnityEngine.UI;

public class EulerAnglesSlideController : SimulationSlideController
{
    [SerializeField] private ClassicEulerAngles eulerAngles;

    [Header("Sliders")]
    [SerializeField] private CustomSlider thetaSlider;
    [SerializeField] private CustomSlider phiSlider;
    [SerializeField] private CustomSlider psiSlider;

    [Header("Reference Objects")]
    [SerializeField] private Toggle referenceToggle;

    public override void InitializeSlide()
    {
        if (eulerAngles)
        {
            if (thetaSlider) eulerAngles.SetTheta(thetaSlider.value);
            if (phiSlider) eulerAngles.SetPhi(phiSlider.value);
            if (psiSlider) eulerAngles.SetPsi(psiSlider.value);
        }

        if (referenceToggle) referenceToggle.isOn = false;
    }
}
