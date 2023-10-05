using UnityEngine;

public class ClassicEulerAngle : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private LineRenderer arcLR;
    [SerializeField] private CircularSector sector;

    public enum Type { Theta, Phi, Psi }
    [Header("Parameters")]
    [SerializeField] private Type type;
    [SerializeField] private float phi;
    [SerializeField] private float theta;
    [SerializeField] private float psi;

    [Header("Settings")]
    [SerializeField, Min(0)] private float radius = 1;
    [SerializeField] private Color color = Color.black;
    [SerializeField, Range(0, 1)] private float sectorAlpha = 1;

    // Local axes of the plane spanning the drawn area
    private Vector3 e1;
    private Vector3 e2;
    // Direction normal to the plane
    private Vector3 e3;

    private void OnValidate()
    {
        ClampAngle(ref phi);
        ClampAngle(ref theta);
        ClampAngle(ref psi);
    }

    public void Redraw()
    {
        float angle = 0;

        // The arc and sector are drawn from e1 towards e2 and orthogonal to e3
        switch (type)
        {
            case Type.Phi:
                angle = phi;
                e1 = Vector3.back;
                e2 = Vector3.right;
                e3 = Vector3.up;
                break;
            case Type.Theta:
                angle = theta;
                e1 = Vector3.up;
                e2 = Quaternion.Euler(0, -phi, 0) * Vector3.left;
                e3 = Quaternion.Euler(0, -phi, 0) * Vector3.back;
                break;
            case Type.Psi:
                angle = psi;
                e1 = Quaternion.Euler(0, -phi, 0) * Vector3.back;
                e3 = Quaternion.Euler(0, -phi, theta) * Vector3.up;
                e2 = Quaternion.Euler(0, -phi, theta) * Vector3.right;
                break;
            default:
                break;
        }

        // Determine the number of degree steps to take in drawing the arc or sector
        int numDegrees = Mathf.RoundToInt(angle);

        // Convert to radians
        angle *= Mathf.Deg2Rad;

        // Draw the arc
        if (arcLR)
        {
            arcLR.positionCount = numDegrees + 1;
            Vector3[] positions = new Vector3[arcLR.positionCount];

            for (int i = 0; i < numDegrees; i++)
            {
                float subangle = i * Mathf.Deg2Rad;
                positions[i] = radius * (Mathf.Cos(subangle) * e1 + Mathf.Sin(subangle) * e2);
            }

            // Add the final position
            positions[numDegrees] = radius * (Mathf.Cos(angle) * e1 + Mathf.Sin(angle) * e2);
            arcLR.SetPositions(positions);

            arcLR.startColor = arcLR.endColor = color;
        }

        // Shade the area
        if (sector)
        {
            sector.SetAngleInRadians(angle);
            sector.SetRadius(radius);
            sector.SetNormal(e3);
            sector.SetE1(e1);
            Color sectorColor = color;
            sectorColor.a = sectorAlpha;
            sector.SetColor(sectorColor);
            sector.Redraw();
        }
    }

    private void ClampAngle(ref float angle)
    {
        if (angle > 360) angle -= 360;
        if (angle < 0) angle += 360;
    }

    public void SetAngles(float phi, float theta, float psi, bool redraw = false)
    {
        this.phi = phi;
        this.theta = theta;
        this.psi = psi;
        if (redraw) Redraw();
    }

    // public void SetValue(float value, bool redraw = false)
    // {
    //     this.value = value;
    //     if (redraw) Redraw();
    // }

    // public void SetTheta(float theta, bool redraw = false)
    // {
    //     this.theta = theta;
    //     if (redraw) Redraw();
    // }

    // public void SetPhi(float phi, bool redraw = false)
    // {
    //     this.phi = phi;
    //     if (redraw) Redraw();
    // }
}
