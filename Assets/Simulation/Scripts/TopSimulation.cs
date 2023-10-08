using System;
using UnityEngine;

public class TopSimulation : Simulation
{
    [SerializeField] private bool autoPlay;

    [Header("Components")]
    [SerializeField] private Transform rod;
    [SerializeField] private Transform disk;
    [SerializeField] private Transform trail;

    [Header("Settings")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField, Tooltip("In meters.")] private float rodLength = 2;
    [SerializeField, Tooltip("In meters.")] private float diskRadius = 1;
    [SerializeField, Tooltip("Normalized disk distance along the rod [0, 1]."), Range(0, 1)]
    private float diskOffset = 0.8f;
    [SerializeField, Tooltip("Whether to draw the trail")] private bool drawTrail;

    [Header("Initial Conditions")]
    [SerializeField, Range(0, 90), Tooltip("Polar angle [deg].")] private float theta0 = 0;
    [SerializeField, Range(-180, 180), Tooltip("Azimuthal angle [deg].")] private float phi0 = 0;
    [SerializeField, Range(0, 360), Tooltip("Spin angle [deg].")] private float psi0 = 0;

    [Header("Solver")]
    [SerializeField, Min(1)] private int numSubsteps = 10;

    [SerializeField] private float thetaDot0; // [deg / s]
    [SerializeField] private float phiDot0; // [deg / s]
    [SerializeField] private float psiDot0 = 1000; // [deg / s]

    private Vector3 currentDirection;

    // ODE variables
    private double[] x;
    private double[] xdot;

    // Moment of inertia
    private double I1; // = I2
    private double I3;

    // Ground constraint
    private double thetaMaxRad;
    private const double TWOPI = 2 * Math.PI;
    private const double DEG2RAD = Math.PI / 180;
    private const double RAD2DEG = 180 / Math.PI;

    public float ThetaMax => (float)(thetaMaxRad * RAD2DEG);

    // Current state of the top (i.e. Euler angles)
    public TopData data;

    private void Awake()
    {
        Initialize();
        UpdateData();

        IsPaused = !autoPlay;
    }

    public void Initialize()
    {
        // Compute the moment of inertia components
        float radiusSquared = diskRadius * diskRadius;
        I1 = 0.25 * radiusSquared + (diskOffset * rodLength) * (diskOffset * rodLength);
        I3 = 0.5 * radiusSquared;
        // Debug.Log("I1 : " + I1 + ", I3 : " + I3);

        // Initialize equations of motion arrays
        x = new double[6] { theta0 * DEG2RAD, phi0 * DEG2RAD, psi0 * DEG2RAD, thetaDot0 * DEG2RAD, phiDot0 * DEG2RAD, psiDot0 * DEG2RAD };
        xdot = new double[6] { thetaDot0 * DEG2RAD, phiDot0 * DEG2RAD, psiDot0 * DEG2RAD, 0, 0, 0 };
        double[] a = ComputeAccelerations();
        xdot[3] = a[0];
        xdot[4] = a[1];
        xdot[5] = a[2];

        // Determine the max polar angle given the top parameters
        thetaMaxRad = 0.5 * Math.PI - Math.Atan2(diskRadius, diskOffset * rodLength);

        // Update the top's appearance
        if (rod)
        {
            Vector3 rodScale = rod.localScale;
            rodScale.y = 0.5f * rodLength;
            rod.localScale = rodScale;
        }
        if (disk)
        {
            Vector3 diskScale = disk.localScale;
            diskScale.x = 2 * diskRadius;
            diskScale.z = 2 * diskRadius;
            disk.localScale = diskScale;
        }

        data = new TopData();
    }

    // Solve the equations of motion
    private void FixedUpdate()
    {
        if (IsPaused) return;

        double deltaTime = Time.fixedDeltaTime / numSubsteps;
        for (int i = 0; i < numSubsteps; i++)
        {
            TakeLeapfrogStep(deltaTime);
        }

        EnforceConstraints();

        UpdateRod();
        UpdateDisk();
        UpdateTrail();
        UpdateData();
    }

    private void UpdateRod()
    {
        if (!rod) return;

        // Get the current angular position of the rod
        float theta = (float)(x[0] * RAD2DEG);
        float phi = (float)(x[1] * RAD2DEG);

        // Restrict the azimuthal angle between [-180, 180)
        phi = (phi + 180f) % 360 - 180;

        // Point the rod in the correct direction
        currentDirection = Quaternion.Euler(0, -phi, -theta) * Vector3.up;
        rod.localPosition = 0.5f * currentDirection * rodLength;
        rod.up = currentDirection;
    }

    private void UpdateDisk()
    {
        if (!disk) return;

        // Get the current spin of the disk
        float psi = (float)(x[2] * RAD2DEG);

        // Restrict the spin angle between [0, 360)
        psi %= 360f;

        // Position the disk and orient it correctly
        disk.localPosition = diskOffset * rodLength * currentDirection;
        disk.up = currentDirection;
        disk.Rotate(Vector3.up, -psi, Space.Self);
    }

    public void Redraw()
    {
        EnforceConstraints();
        UpdateRod();
        UpdateDisk();
    }

    private void UpdateTrail()
    {
        if (trail && drawTrail) trail.localPosition = rodLength * currentDirection;
    }

    private void EnforceConstraints()
    {
        // Check for whether the top has fallen
        if (Math.Abs(x[0]) >= thetaMaxRad)
        {
            // theta
            x[0] = Math.Sign(x[0]) * thetaMaxRad;
            x[3] = 0;
            xdot[0] = 0;
            xdot[3] = 0;

            // TODO fix what happens when the top falls while running

            // phi
            xdot[4] = 0;

            // psi
            x[5] = x[4];
            xdot[2] = xdot[1];
            xdot[5] = 0;
        }
    }

    private double[] ComputeAccelerations()
    {
        double sin = Math.Sin(x[0]);
        double cos = Math.Cos(x[0]);
        double r = I3 / I1;
        double torque = 1 * gravity * (diskOffset * rodLength) * sin; // Assuming mass = 1

        // Euler equations of motion
        double aTheta = (1 - r) * xdot[1] * xdot[1] * sin * cos - r * xdot[1] * xdot[2] * sin + torque / I1;
        double aPhi = r * xdot[0] * (xdot[2] + xdot[1] * cos) / sin - 2 * xdot[0] * xdot[1] * cos / sin;
        double aPsi = xdot[0] * xdot[1] * sin - aPhi * cos;

        return new double[3] { aTheta, aPhi, aPsi };
    }

    private void TakeLeapfrogStep(double deltaTime)
    {
        // Update positions with current velocities and accelerations
        x[0] += deltaTime * (xdot[0] + 0.5 * xdot[3] * deltaTime);
        x[1] += deltaTime * (xdot[1] + 0.5 * xdot[4] * deltaTime);
        x[2] += deltaTime * (xdot[2] + 0.5 * xdot[5] * deltaTime);

        x[0] = LoopAngle(x[0]);
        x[1] = LoopAngle(x[1]);
        x[2] = LoopAngle(x[2]);

        // Compute new accelerations and update velocities
        double[] aNew = ComputeAccelerations();
        x[3] += 0.5 * (xdot[3] + aNew[0]) * deltaTime;
        x[4] += 0.5 * (xdot[4] + aNew[1]) * deltaTime;
        x[5] += 0.5 * (xdot[5] + aNew[2]) * deltaTime;

        // Update accelerations
        xdot[0] = x[3];
        xdot[1] = x[4];
        xdot[2] = x[5];
        xdot[3] = aNew[0];
        xdot[4] = aNew[1];
        xdot[5] = aNew[2];
    }

    private double LoopAngle(double value)
    {
        double result = value;
        if (value > TWOPI) result -= TWOPI;
        if (value < 0) result += TWOPI;
        return result;
    }

    private Vector3 SphericalToCartesian(float a1, float a2, float a3)
    {
        // (a1, a2, a3) correspond to the (r, theta, phi) components of a vector in spherical basis

        float theta = (float)x[0];
        float phi = (float)x[1];
        // float psi = (float)x[2];
        float sinTheta = Mathf.Sin(theta);
        float cosTheta = Mathf.Cos(theta);
        float sinPhi = Mathf.Sin(phi);
        float cosPhi = Mathf.Cos(phi);

        float ax = a1 * sinTheta * cosPhi + a2 * cosTheta * cosPhi - a3 * sinPhi;
        float ay = a1 * sinTheta * sinPhi + a2 * cosTheta * sinPhi + a3 * cosPhi;
        float az = a1 * cosTheta - a2 * sinTheta;

        // Account for Unity's left-handedness and axis orientation
        return new Vector3(ax, az, ay);
    }

    public Vector3 AngularVelocity()
    {
        float theta = (float)x[0];
        float thetaDot = (float)x[3];
        float phiDot = (float)x[4];
        float psiDot = (float)x[5];

        return SphericalToCartesian(psiDot + phiDot * Mathf.Cos(theta), -phiDot * Mathf.Sin(theta), thetaDot);
    }

    public Vector3 AngularMomentum()
    {
        float theta = (float)x[0];
        float thetaDot = (float)x[3];
        float phiDot = (float)x[4];
        float psiDot = (float)x[5];

        return SphericalToCartesian((float)I3 * (psiDot + phiDot * Mathf.Cos(theta)), -(float)I1 * phiDot * Mathf.Sin(theta), (float)I1 * thetaDot);
    }

    private void UpdateData()
    {
        data.theta = (float)(x[0] * RAD2DEG);
        data.phi = (float)(x[1] * RAD2DEG);
        data.psi = (float)(x[2] * RAD2DEG);
    }

    public void SetTheta0(float theta0)
    {
        // Only set if the simulation is not running
        if (!IsPaused) return;

        this.theta0 = theta0;
        Initialize();
        Redraw();
    }

    public void SetPhi0(float phi0)
    {
        // Only set if the simulation is not running
        if (!IsPaused) return;

        this.phi0 = phi0;
        Initialize();
        Redraw();
    }

    public void SetPsi0(float psi0)
    {
        // Only set if the simulation is not running
        if (!IsPaused) return;

        this.psi0 = psi0;
        Initialize();
        Redraw();
    }

    public void SetInitialEulerAngles(float theta, float phi, float psi)
    {
        theta0 = theta;
        phi0 = phi;
        psi0 = psi;
        Initialize();
        Redraw();
    }
}

public class TopData
{
    public float theta;
    public float phi;
    public float psi;
}
