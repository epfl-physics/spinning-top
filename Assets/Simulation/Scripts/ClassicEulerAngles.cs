using UnityEngine;

public class ClassicEulerAngles : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float phi;
    [SerializeField] private float theta;
    [SerializeField] private float psi;
    [SerializeField] private bool broadcastValues;

    [Header("Euler Angle Displays")]
    [SerializeField] private ClassicEulerAngle phiDisplay;
    [SerializeField] private ClassicEulerAngle thetaDisplay;
    [SerializeField] private ClassicEulerAngle psiDisplay;

    [Header("Relative Basis")]
    [SerializeField] private float basisLength = 1;
    [SerializeField] private Vector y1Vector;
    [SerializeField] private Vector y2Vector;
    [SerializeField] private Vector y3Vector;

    [Header("Reference Object")]
    [SerializeField] private Transform referenceObject;
    [SerializeField] private float initialRotationZ = 0;

    [Header("Simulation Data")]
    [SerializeField] private TopSimulationState simState;

    public static event System.Action<float, float, float> OnValuesChanged;

    // private void OnEnable()
    // {
    //     TopSimulationState.OnUpdateData += HandleTopUpdated;
    // }

    // private void OnDisable()
    // {
    //     TopSimulationState.OnUpdateData -= HandleTopUpdated;
    // }

    private void Start()
    {
        Redraw();
    }

    public void SetPhi(float value)
    {
        phi = value;
        Redraw();
    }

    public void SetTheta(float value)
    {
        theta = value;
        Redraw();
    }

    public void SetPsi(float value)
    {
        psi = value;
        Redraw();
    }

    public void Redraw()
    {
        // Initial relative basis vectors
        Vector3 x1 = basisLength * Vector3.back;
        Vector3 x2 = basisLength * Vector3.right;
        Vector3 x3 = basisLength * Vector3.up;

        // Rotate the basis
        Quaternion rotation = Quaternion.AngleAxis(initialRotationZ, Vector3.forward);
        Vector3 y1 = Quaternion.AngleAxis(-phi, Vector3.up) * x1;
        Vector3 y2 = Quaternion.AngleAxis(-phi, Vector3.up) * x2;
        rotation = Quaternion.AngleAxis(-phi, Vector3.up) * rotation;
        Vector3 y3 = Quaternion.AngleAxis(-theta, y1) * x3;
        y2 = Quaternion.AngleAxis(-theta, y1) * y2;
        rotation = Quaternion.AngleAxis(-theta, y1) * rotation;
        y1 = Quaternion.AngleAxis(-psi, y3) * y1;
        y2 = Quaternion.AngleAxis(-psi, y3) * y2;
        rotation = Quaternion.AngleAxis(-psi, y3) * rotation;

        // Redraw the Vectors
        if (y1Vector)
        {
            y1Vector.components = y1;
            y1Vector.Redraw();
            y1Vector.gameObject.SetActive(y1 != x1);
        }
        if (y2Vector)
        {
            y2Vector.components = y2;
            y2Vector.Redraw();
            y2Vector.gameObject.SetActive(y2 != x2);
        }
        if (y3Vector)
        {
            y3Vector.components = y3;
            y3Vector.Redraw();
            y3Vector.gameObject.SetActive(y3 != x3);
        }

        // Euler angle displays
        if (phiDisplay) phiDisplay.SetAngles(phi, theta, psi, true);
        if (thetaDisplay) thetaDisplay.SetAngles(phi, theta, psi, true);
        if (psiDisplay) psiDisplay.SetAngles(phi, theta, psi, true);

        // Update the reference object's orientation
        if (referenceObject) referenceObject.rotation = rotation;

        // Broadcast values
        if (broadcastValues) OnValuesChanged?.Invoke(theta, phi, psi);
    }

    public void Reset()
    {
        theta = 0;
        phi = 0;
        psi = 0;
        Redraw();
    }

    // public void HandleTopUpdated()
    // {
    //     if (simState)
    //     {
    //         theta = simState.data.theta;
    //         phi = simState.data.phi;
    //         phi = (phi + 180) % 360;
    //         psi = simState.SimIsRunning ? 360 : simState.data.psi;
    //         // Redraw();
    //         Debug.Log("What was I doing here?");
    //     }
    // }
}
