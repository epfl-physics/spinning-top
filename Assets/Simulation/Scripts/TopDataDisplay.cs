using TMPro;
using UnityEngine;

public class TopDataDisplay : MonoBehaviour
{
    [SerializeField] private TopSimulationState simState;
    [SerializeField] private TextMeshProUGUI thetaValue;
    [SerializeField] private TextMeshProUGUI phiValue;
    [SerializeField] private TextMeshProUGUI psiValue;

    [SerializeField] private TextMeshProUGUI angularMomentumValue;
    [SerializeField] private TextMeshProUGUI angularVelocityValue;
    [SerializeField] private TextMeshProUGUI energyRatioValue;

    [SerializeField] private TextMeshProUGUI fpsValue;

    // private void Start()
    // {
    //     gameObject.SetActive(false);
    // }

    private void Update()
    {
        if (!simState) return;

        if (thetaValue) thetaValue.text = simState.data.theta.ToString("0.0") + "˚";
        if (phiValue) phiValue.text = simState.data.phi.ToString("0.0") + "˚";
        if (psiValue) psiValue.text = simState.data.psi.ToString("0.0") + "˚";

        if (angularMomentumValue) angularMomentumValue.text = simState.data.angularMomentum.magnitude.ToString("0.00");
        if (angularVelocityValue) angularVelocityValue.text = simState.data.angularVelocity.magnitude.ToString("0.00");
        if (energyRatioValue) energyRatioValue.text = simState.data.energyRatio.ToString("0.00");

        if (fpsValue) fpsValue.text = (Time.frameCount / Time.time).ToString("0.0");
    }
}
