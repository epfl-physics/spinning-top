using UnityEngine;

public class EulerAngle : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private LineRenderer referenceLR;
    [SerializeField] private LineRenderer directionLR;
    [SerializeField] private LineRenderer arcLR;
    [SerializeField] private CircularSector sector;
    private Vector3 referenceAxis;

    public enum Type { Theta, Phi, Psi }
    [Header("Parameters")]
    [SerializeField] private Type type;
    [SerializeField] private float value;
    [SerializeField] private float theta;
    [SerializeField] private float phi;

    [Header("Settings")]
    [SerializeField, Min(0)] private float axisLength = 2;
    [SerializeField, Min(0)] private float radius = 1;
    [SerializeField] private Color color = Color.black;
    [SerializeField, Range(0, 1)] private float sectorAlpha = 1;

    private Vector3 direction;

    private void OnValidate()
    {
        theta = Mathf.Clamp(theta, 0, 90);
        if (phi > 360) phi -= 360;
        if (phi < 0) phi += 360;
    }

    public void Redraw()
    {
        if (referenceLR) referenceLR.positionCount = 0;
        if (directionLR) directionLR.positionCount = 0;
        if (arcLR) arcLR.positionCount = 0;

        int numDegrees = Mathf.RoundToInt(value);
        arcLR.positionCount = numDegrees + 1;
        Vector3[] positions = new Vector3[arcLR.positionCount];

        // Local axes of the plane spanning the reference axis and direction
        Vector3 e1;
        Vector3 e2;

        switch (type)
        {
            case Type.Theta:
                referenceAxis = Vector3.up;
                value = Mathf.Clamp(value, 0, 90);
                referenceLR.positionCount = 2;
                referenceLR.SetPositions(new Vector3[2] { Vector3.zero, axisLength * Vector3.up });
                direction = Quaternion.Euler(0, -phi, -value) * Vector3.up;
                if (direction != referenceAxis)
                {
                    directionLR.positionCount = 2;
                    directionLR.SetPositions(new Vector3[2] { Vector3.zero, axisLength * direction });

                    for (int i = 0; i < numDegrees; i++)
                    {
                        positions[i] = axisLength * (Quaternion.Euler(0, -phi, -i) * Vector3.up);
                    }
                    positions[numDegrees] = axisLength * direction;
                    arcLR.SetPositions(positions);
                }
                if (sector)
                {
                    sector.SetAngle(value);
                    sector.SetRadius(axisLength);
                    sector.SetNormal(-Vector3.Cross(Vector3.up, direction).normalized);
                    sector.SetE1(Vector3.up);
                    Color sectorColor = color;
                    sectorColor.a = sectorAlpha;
                    sector.SetColor(sectorColor);
                    sector.Redraw();
                }
                break;
            case Type.Phi:
                referenceAxis = Vector3.right;
                if (value > 360) value -= 360;
                if (value < 0) value += 360;
                referenceLR.positionCount = 2;
                referenceLR.SetPositions(new Vector3[2] { Vector3.zero, axisLength * Vector3.right });
                direction = Quaternion.Euler(0, -value, 0) * Vector3.right;

                e1 = Vector3.right;
                e2 = Vector3.forward;
                if (direction != referenceAxis)
                {
                    directionLR.positionCount = 2;
                    directionLR.SetPositions(new Vector3[2] { Vector3.zero, axisLength * direction });
                }
                for (int i = 0; i < numDegrees; i++)
                {
                    // positions[i] = axisLength * (Quaternion.Euler(0, -i, 0) * Vector3.right);
                    float angle = i * Mathf.Deg2Rad;
                    positions[i] = axisLength * (Mathf.Cos(angle) * e1 + Mathf.Sin(angle) * e2);
                }
                positions[numDegrees] = axisLength * direction;
                arcLR.SetPositions(positions);
                if (sector)
                {
                    sector.SetAngle(value);
                    sector.SetRadius(axisLength);
                    sector.SetNormal(Vector3.up);
                    sector.SetE1(Vector3.right);
                    Color sectorColor = color;
                    sectorColor.a = sectorAlpha;
                    sector.SetColor(sectorColor);
                    sector.Redraw();
                }
                break;
            case Type.Psi:
                referenceAxis = Quaternion.Euler(0, -phi, -theta) * Vector3.up;
                if (value > 360) value -= 360;
                if (value < 0) value += 360;
                referenceLR.positionCount = 2;
                referenceLR.SetPositions(new Vector3[2] { Vector3.zero, axisLength * referenceAxis });

                e1 = Vector3.forward;
                e2 = Vector3.left;
                if (referenceAxis != Vector3.up)
                {
                    // Local e1 axis parallel to the ground
                    e1 = -Vector3.Cross(Vector3.up, referenceAxis).normalized;
                    // Local e2 axis
                    e2 = -Vector3.Cross(referenceAxis, e1).normalized;
                }
                for (int i = 0; i < numDegrees; i++)
                {
                    float angle = i * Mathf.Deg2Rad;
                    positions[i] = axisLength * referenceAxis + radius * (Mathf.Cos(angle) * e1 + Mathf.Sin(angle) * e2);
                }

                positions[numDegrees] = axisLength * referenceAxis + radius * (Mathf.Cos(Mathf.Deg2Rad * value) * e1 + Mathf.Sin(Mathf.Deg2Rad * value) * e2);
                arcLR.SetPositions(positions);

                if (sector)
                {
                    sector.transform.position = axisLength * referenceAxis;
                    sector.SetAngle(value);
                    sector.SetRadius(radius);
                    sector.SetNormal(referenceAxis.normalized);
                    sector.SetE1(e1);
                    Color sectorColor = color;
                    sectorColor.a = sectorAlpha;
                    sector.SetColor(sectorColor);
                    sector.Redraw();
                }
                break;
            default:
                break;
        }

        arcLR.startColor = color;
        arcLR.endColor = color;
    }

    public void SetValue(float value, bool redraw = false)
    {
        this.value = value;
        if (redraw) Redraw();
    }

    public void SetTheta(float theta, bool redraw = false)
    {
        this.theta = theta;
        if (redraw) Redraw();
    }

    public void SetPhi(float phi, bool redraw = false)
    {
        this.phi = phi;
        if (redraw) Redraw();
    }
}
