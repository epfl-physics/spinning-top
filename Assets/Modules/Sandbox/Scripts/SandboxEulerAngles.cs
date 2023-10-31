using UnityEngine;

public class SandboxEulerAngles : MonoBehaviour
{
    [Header("Euler Angle Visualisations")]
    [SerializeField] private ClassicEulerAngle eulerPhi;
    [SerializeField] private ClassicEulerAngle eulerTheta;
    [SerializeField] private ClassicEulerAngle eulerPsi;
    [SerializeField] private bool psiIsStatic;

    [Header("Simulation Data")]
    [SerializeField] private TopSimulationState simState;

    private void OnEnable()
    {
        TopSimulationState.OnUpdateData += HandleTopDataUpdated;
    }

    private void OnDisable()
    {
        TopSimulationState.OnUpdateData -= HandleTopDataUpdated;
    }

    public void HandleTopDataUpdated()
    {
        if (!simState) return;

        float phi = simState.data.phi;
        float theta = simState.data.theta;
        float psi = psiIsStatic ? 360 : simState.data.psi;

        if (eulerPhi) eulerPhi.SetAngles(phi, theta, psi, true);
        if (eulerTheta) eulerTheta.SetAngles(phi, theta, psi, true);
        if (eulerPsi) eulerPsi.SetAngles(phi, theta, psi, true);
    }
}
