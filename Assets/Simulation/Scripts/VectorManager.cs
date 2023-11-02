using UnityEngine;

public class VectorManager : MonoBehaviour
{
    // [SerializeField] private VectorDisplay[] vectorDisplays;

    [Header("Angular Velocity")]
    [SerializeField] private Vector thetaDot;
    [SerializeField] private Vector phiDot;
    [SerializeField] private Vector psiDot;
    [SerializeField] private Vector omega;

    [Header("Dynamics")]
    [SerializeField] private Vector angularMomentum;
    [SerializeField] private Vector torque;
    [SerializeField] private Vector weight;

    [Header("Basis")]
    [SerializeField] private Transform relativeBasis;

    [Header("Simulation Data")]
    [SerializeField] private TopSimulationState simState;

    private void LateUpdate()
    {
        if (!simState) return;

        RedrawVector(thetaDot, simState.data.thetaDot * simState.data.UHat, 0.1f);
        RedrawVector(phiDot, simState.data.phiDot * Vector3.up, 0.05f);
        RedrawVector(psiDot, simState.data.psiDot * simState.data.Y3Hat, 0.0015f);
        RedrawVector(omega, simState.data.angularVelocity, 0.0015f);
        RedrawVector(angularMomentum, simState.data.angularMomentum, 0.0015f);
        RedrawVector(torque, simState.data.torque, 0.2f);
        Vector3 weightPosition = simState.data.diskOffset * simState.data.Direction;
        Vector3 weightComponents = simState.data.diskMass * simState.data.gravity * Vector3.down;
        RedrawVector(weight, weightPosition, weightComponents, 1);

        if (relativeBasis)
        {
            relativeBasis.position = simState.data.diskOffset * simState.data.Direction;
            relativeBasis.rotation = Quaternion.Euler(0, -simState.data.phi, simState.data.theta);
        }
    }

    private void RedrawVector(Vector vector, Vector3 components, float scaleFactor = 1)
    {
        if (vector)
        {
            vector.components = scaleFactor * components;
            vector.Redraw();
        }
    }

    private void RedrawVector(Vector vector, Vector3 position, Vector3 components, float scaleFactor = 1)
    {
        if (vector) vector.transform.position = position;
        RedrawVector(vector, components, scaleFactor);
    }
}

// [System.Serializable]
// public class VectorDisplay
// {
//     public enum Type { ThetaDot, PhiDot, PsiDot, AngularVelocity, AngularMomentum, Torque }
//     public Type type;
//     public Vector vector;
//     public float scaleFactor = 1;
// }
