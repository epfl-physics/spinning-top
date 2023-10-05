using UnityEngine;

public class EulerAngles : MonoBehaviour
{
    [SerializeField] private TopSimulation sim;

    [Header("Sliders")]
    [SerializeField] private CustomSlider thetaSlider;
    [SerializeField] private CustomSlider phiSlider;
    [SerializeField] private CustomSlider psiSlider;

    [Header("Euler Angles")]
    [SerializeField] private EulerAngle theta;
    [SerializeField] private EulerAngle phi;
    [SerializeField] private EulerAngle psi;

    private void Start()
    {
        if (!sim) return;

        float theta0 = thetaSlider ? thetaSlider.value : 0;
        float phi0 = phiSlider ? phiSlider.value : 0;
        float psi0 = psiSlider ? psiSlider.value : 0;

        // Make sure sim matches initial slider values
        sim.SetInitialEulerAngles(theta0, phi0, psi0);

        if (theta)
        {
            theta.SetPhi(phi0);
            theta.SetValue(theta0, true);
        }
        if (phi)
        {
            phi.SetValue(phi0, true);
        }
        if (psi)
        {
            psi.SetTheta(theta0);
            psi.SetPhi(phi0);
            psi.SetValue(psi0, true);
        }

        if (thetaSlider) thetaSlider.maxValue = sim.ThetaMax;

        // if (phiSlider) phiSlider.value = sim.data.phi;
        // if (psiSlider) psiSlider.value = sim.data.psi;
        // if (thetaSlider) thetaSlider.value = sim.data.theta;

    }

    public void SetTheta(float value)
    {
        Debug.Log("EulerAngles > SetTheta");
        if (theta) theta.SetValue(value, true);
        if (psi) psi.SetTheta(value, true);

        if (sim) sim.SetInitialEulerAngles(value, phiSlider.value, psiSlider.value);
    }

    public void SetPhi(float value)
    {
        Debug.Log("EulerAngles > SetPhi");
        if (phi) phi.SetValue(value, true);
        if (theta) theta.SetPhi(value, true);
        if (psi) psi.SetPhi(value, true);

        if (sim) sim.SetInitialEulerAngles(thetaSlider.value, value, psiSlider.value);
    }

    public void SetPsi(float value)
    {
        Debug.Log("EulerAngles > SetPsi");
        if (psi) psi.SetValue(value, true);

        if (sim) sim.SetInitialEulerAngles(thetaSlider.value, phiSlider.value, value);
    }
}
