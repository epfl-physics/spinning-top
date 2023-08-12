using TMPro;
using UnityEngine;

public class VectorManager : MonoBehaviour
{
    [SerializeField] private TopSimulation sim;
    [SerializeField] private Vector angularVelocityVector;
    [SerializeField] private TextMeshProUGUI angularVelocityValue;
    [SerializeField] private Vector angularMomentumVector;
    [SerializeField] private TextMeshProUGUI angularMomentumValue;

    [SerializeField] private float scaleFactor = 1;

    private void LateUpdate()
    {
        if (!sim) return;

        if (angularVelocityVector)
        {
            Vector3 angularVelocity = sim.AngularVelocity();
            angularVelocityVector.components = scaleFactor * angularVelocity.normalized;
            angularVelocityVector.Redraw();

            if (angularVelocityValue)
            {
                angularVelocityValue.text = angularVelocity.magnitude.ToString("0.000");
            }
        }

        if (angularMomentumVector)
        {
            Vector3 angularMomentum = sim.AngularMomentum();
            angularMomentumVector.components = scaleFactor * angularMomentum.normalized;
            angularMomentumVector.Redraw();

            if (angularMomentumValue)
            {
                angularMomentumValue.text = angularMomentum.magnitude.ToString("0.000");
            }
        }
    }
}
