using Slides;
using UnityEngine;

public class EulerAnglesSlideController : SimulationSlideController
{
    [SerializeField] private ClassicEulerAngles eulerAngles;

    [Header("Sliders")]
    [SerializeField] private CustomSlider thetaSlider;
    [SerializeField] private CustomSlider phiSlider;
    [SerializeField] private CustomSlider psiSlider;

    public override void InitializeSlide()
    {
        if (eulerAngles)
        {
            if (thetaSlider) eulerAngles.SetTheta(thetaSlider.value);
            if (phiSlider) eulerAngles.SetPhi(phiSlider.value);
            if (psiSlider) eulerAngles.SetPsi(psiSlider.value);
        }
    }
}
