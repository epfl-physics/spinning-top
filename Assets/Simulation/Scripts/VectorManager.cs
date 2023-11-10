using UnityEngine;

public class VectorManager : MonoBehaviour
{
    [SerializeField] private bool autoUpdate = true;

    [Header("Simulation Data")]
    [SerializeField] private TopSimulationState simState;

    [Header("Vectors")]
    public VectorDisplay[] vectorDisplays;

    [Header("Basis")]
    [SerializeField] private Transform relativeBasis;
    [SerializeField] private GameObject x3Hat;

    private Vector3 GetVectorComponents(VectorDisplay vectorDisplay)
    {
        Vector3 components = Vector3.zero;

        if (!simState) return components;

        switch (vectorDisplay.type)
        {
            case VectorDisplay.Type.ThetaDot:
                components = Mathf.Deg2Rad * simState.data.thetaDot * simState.data.UHat;
                break;
            case VectorDisplay.Type.PhiDot:
                components = Mathf.Deg2Rad * simState.data.phiDot * Vector3.up;
                break;
            case VectorDisplay.Type.PsiDot:
                components = Mathf.Deg2Rad * simState.data.psiDot * simState.data.Y3Hat;
                break;
            case VectorDisplay.Type.AngularVelocity:
                components = simState.data.angularVelocity;
                break;
            case VectorDisplay.Type.AngularMomentum:
                components = simState.data.angularMomentum;
                break;
            case VectorDisplay.Type.Torque:
                components = simState.data.torque;
                break;
            case VectorDisplay.Type.Weight:
                components = simState.data.diskMass * simState.data.gravity * Vector3.down;
                break;
            default:
                break;
        }

        return components;
    }

    private void LateUpdate()
    {
        if (!autoUpdate) return;
        if (!simState) return;

        VectorDisplay phiDot = null;

        foreach (VectorDisplay vectorDisplay in vectorDisplays)
        {
            Vector3 components = GetVectorComponents(vectorDisplay);
            if (vectorDisplay.type == VectorDisplay.Type.Weight)
            {
                Vector3 position = simState.data.diskOffset * simState.data.Direction;
                RedrawVector(vectorDisplay.vector, position, components, vectorDisplay.scaleFactor);
            }
            else
            {
                RedrawVector(vectorDisplay.vector, components, vectorDisplay.scaleFactor);
                if (vectorDisplay.type == VectorDisplay.Type.PhiDot) phiDot = vectorDisplay;
            }
        }

        if (relativeBasis)
        {
            relativeBasis.position = simState.data.diskOffset * simState.data.Direction;
            relativeBasis.rotation = Quaternion.Euler(0, -simState.data.phi, simState.data.theta);
        }

        if (x3Hat && phiDot != null)
        {
            if (phiDot.vector.gameObject.activeInHierarchy)
            {
                x3Hat.SetActive(phiDot.vector.components.y < -0.1f);
            }
            else if (!x3Hat.activeInHierarchy)
            {
                x3Hat.SetActive(true);
            }
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

[System.Serializable]
public class VectorDisplay
{
    public enum Type { ThetaDot, PhiDot, PsiDot, AngularVelocity, AngularMomentum, Torque, Weight }
    public Type type;
    public Vector vector;
    public float scaleFactor = 1;
}
