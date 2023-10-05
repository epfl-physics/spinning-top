using TMPro;
using UnityEngine;

public class TopDataDisplay : MonoBehaviour
{
    [SerializeField] private TopSimulation sim;
    [SerializeField] private TextMeshProUGUI thetaValue;
    [SerializeField] private TextMeshProUGUI phiValue;
    [SerializeField] private TextMeshProUGUI psiValue;

    [SerializeField] private TextMeshProUGUI angularMomentumValue;
    [SerializeField] private TextMeshProUGUI angularVelocityValue;

    private void Update()
    {
        if (!sim) return;

        if (thetaValue) thetaValue.text = sim.data.theta.ToString("0.0") + "˚";
        if (phiValue) phiValue.text = sim.data.phi.ToString("0.0") + "˚";
        if (psiValue) psiValue.text = sim.data.psi.ToString("0.0") + "˚";

        if (angularMomentumValue) angularMomentumValue.text = sim.AngularMomentum().magnitude.ToString("0.000");
        if (angularVelocityValue) angularVelocityValue.text = sim.AngularVelocity().magnitude.ToString("0.000");
    }
}
