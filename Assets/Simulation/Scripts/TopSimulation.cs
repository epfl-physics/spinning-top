using System;
using UnityEngine;

public class TopSimulation : Simulation
{
    [SerializeField] private TopSimulationState simState;
    [SerializeField] private bool autoPlay;

    [Header("Components")]
    [SerializeField] private Transform rod;
    [SerializeField] private Transform disk;
    [SerializeField] private Transform trail;

    [Header("Settings")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField, Tooltip("In meters.")] private float rodLength = 2;
    [SerializeField, Tooltip("In meters.")] private float diskRadius = 1;
    [SerializeField, Tooltip("In kilograms.")] private float diskMass = 1;
    [SerializeField, Tooltip("Disk distance along the rod in meters.")] private float diskOffset = 2.4f;
    [SerializeField, Tooltip("Whether to draw the trail")] private bool drawTrail;

    [Header("Initial Conditions")]
    [SerializeField, Range(0, 180), Tooltip("Nutation angle [deg].")] private float theta0 = 0;
    [SerializeField, Range(-180, 180), Tooltip("Precession angle [deg].")] private float phi0 = 0;
    [SerializeField, Range(0, 360), Tooltip("Intrinsic rotation [deg].")] private float psi0 = 0;
    [SerializeField, Range(-10, 10), Tooltip("d(theta)/dt [deg / s]")] private float thetaDot0 = 0; // [deg / s]
    [SerializeField, Range(-50, 50), Tooltip("d(phi)/dt [deg / s]")] private float phiDot0 = 0; // [deg / s]
    [SerializeField, Range(-4000, 4000), Tooltip("d(psi)/dt [deg / s]")] private float psiDot0 = 2000; // [deg / s]

    [Header("Solver")]
    [SerializeField, Min(1)] private int numSubsteps = 10;
    private enum Solver { Leapfrog, Yoshida }
    [SerializeField] private Solver solver = Solver.Leapfrog;

    // Current state of the top (i.e. Euler angles, derivatives, moment of inertia)
    private TopData data;

    // ODE variables
    private double[] x;
    private double[] v;
    private double[] a;

    // Moment of inertia components
    private double I1; // = I2
    private double I3;

    // Conversion factors
    private static readonly double TWOPI = 2 * Math.PI;
    private static readonly double DEG2RAD = Math.PI / 180;
    private static readonly double RAD2DEG = 180 / Math.PI;

    // Yoshida constants
    private static readonly double w1 = 1.0 / (2.0 - Math.Pow(2.0, 1.0 / 3.0));
    private static readonly double w0 = -Math.Pow(2.0, 1.0 / 3.0) * w1;
    private static readonly double[] c = { 0.5 * w1, 0.5 * (w0 + w1), 0.5 * (w0 + w1), 0.5 * w1 };
    private static readonly double[] d = { w1, w0, w1 };

    // Ground constraint
    private double thetaMaxRad;
    public float ThetaMax => (float)(thetaMaxRad * RAD2DEG);

    public static event Action OnTopHasFallen;
    public static event Action OnTopHasBecomeUnstable;

    private void Awake()
    {
        // Reset the simulation and pause
        Reset();

        // Potentially start playing
        if (autoPlay) Resume();
    }

    public void ComputeMomentOfInertia()
    {
        // Compute the moment of inertia components (assume the rod is massless)
        float radiusSquared = diskRadius * diskRadius;
        I1 = 0.25 * diskMass * radiusSquared + diskOffset * diskOffset;
        I3 = 0.5 * diskMass * radiusSquared;
    }

    private void InitializeODE()
    {
        // Initialize the equations of motion using the initial conditions
        x = new double[3] { theta0 * DEG2RAD, phi0 * DEG2RAD, psi0 * DEG2RAD };
        v = new double[3] { thetaDot0 * DEG2RAD, phiDot0 * DEG2RAD, psiDot0 * DEG2RAD };
        a = new double[3];
        ComputeAccelerations();
    }

    private void ComputeMaxPolarAngle()
    {
        // Determine the maximum polar angle given the top parameters ensuring that 
        // the top does not pass through the floor
        thetaMaxRad = 0.5 * Math.PI - Math.Atan2(diskRadius, diskOffset);
    }

    private void UpdatePhysicalAppearance()
    {
        // Update the top's appearance to correspond with its settings
        if (rod)
        {
            Vector3 rodScale = rod.localScale;
            rodScale.y = 0.5f * rodLength;
            rod.localScale = rodScale;
        }

        if (disk)
        {
            // Distance along the rod
            disk.localPosition = diskOffset * data.Direction;
            // Disk size
            Vector3 diskScale = disk.localScale;
            diskScale.x = 2 * diskRadius;
            diskScale.z = 2 * diskRadius;
            disk.localScale = diskScale;
        }
    }

    private void UpdateOrientation()
    {
        // Orient the top according to the values stored in data
        if (rod)
        {
            rod.localPosition = 0.5f * rodLength * data.Direction;
            rod.up = data.Direction;
        }

        if (disk)
        {
            disk.localPosition = diskOffset * data.Direction;
            disk.rotation = Quaternion.Euler(0, -data.phi, data.theta);
            disk.Rotate(Vector3.up, -data.psi, Space.Self);
        }
    }

    // Solve the equations of motion
    private void FixedUpdate()
    {
        if (IsPaused) return;

        // Evolve the equations of motion in time
        double deltaTime = Time.fixedDeltaTime / numSubsteps;
        for (int i = 0; i < numSubsteps; i++)
        {
            if (x[0] == 0)
                RotateAroundY(deltaTime);
            else if (solver == Solver.Leapfrog)
                TakeLeapfrogStep(deltaTime);
            else if (solver == Solver.Yoshida)
                TakeYoshidaStep(deltaTime);
        }

        UpdateData();
        UpdateSimState(true);
        UpdateOrientation();
    }

    // private void UpdateTrail()
    // {
    //     if (trail && drawTrail) trail.localPosition = rodLength * data.Direction;
    // }

    private void ComputeAccelerations()
    {
        double sin = Math.Sin(x[0]);
        double cos = Math.Cos(x[0]);
        double r = I3 / I1;
        double torque = diskMass * gravity * diskOffset * sin;

        // Euler equations of motion
        a[0] = (1 - r) * v[1] * v[1] * sin * cos - r * v[1] * v[2] * sin + torque / I1;
        a[1] = r * v[0] * (v[2] + v[1] * cos) / sin - 2 * v[0] * v[1] * cos / sin;
        a[2] = v[0] * v[1] * sin - a[1] * cos;
    }

    private void RotateAroundY(double deltaTime)
    {
        // Handle the special case of the top pointing straight up or down
        x[2] += deltaTime * v[2];
        x[2] = WrapAngle(x[2]);
    }

    private void TakeLeapfrogStep(double deltaTime)
    {
        // Half kick
        for (int i = 0; i < 3; i++)
        {
            v[i] += 0.5 * deltaTime * a[i];
        }

        // Drift
        for (int i = 0; i < 3; i++)
        {
            x[i] += deltaTime * v[i];
            x[i] = WrapAngle(x[i]);
        }

        ComputeAccelerations();

        // Half kick
        for (int i = 0; i < 3; i++)
        {
            v[i] += 0.5 * deltaTime * a[i];
        }
    }

    private void TakeYoshidaStep(double deltaTime)
    {
        for (int i = 0; i < 3; i++)
        {
            // Drift
            for (int j = 0; j < 3; j++)
            {
                x[j] += c[i] * deltaTime * v[j];
            }

            // Compute Accelerations
            ComputeAccelerations();

            // Kick
            for (int j = 0; j < 3; j++)
            {
                v[j] += d[i] * deltaTime * a[j];
            }
        }

        // Final drift
        for (int j = 0; j < 3; j++)
        {
            x[j] = WrapAngle(x[j] + c[3] * deltaTime * v[j]);
        }
    }

    private double WrapAngle(double value)
    {
        double result = value;
        if (value > TWOPI) result -= TWOPI;
        if (value < 0) result += TWOPI;
        return result;
    }

    private float ComputeEnergy(Vector3 e1, Vector3 e2, float sin, float cos)
    {
        float rotationK = 0.5f * Vector3.Dot(Mathf.Deg2Rad * data.angularVelocity, Mathf.Deg2Rad * data.angularMomentum);
        Vector3 vTranslation = diskOffset * Mathf.Deg2Rad * (data.phiDot * sin * e1 - data.thetaDot * e2);
        float translationK = 0.5f * diskMass * vTranslation.sqrMagnitude;
        float potential = diskMass * gravity * (rodLength + diskOffset * cos);
        return rotationK + translationK + potential;
    }

    private void UpdateData()
    {
        data.gravity = gravity;

        data.diskRadius = diskRadius;
        data.diskOffset = diskOffset;
        data.diskMass = diskMass;
        data.I1 = (float)I1;
        data.I3 = (float)I3;

        data.theta = (float)(x[0] * RAD2DEG);
        data.phi = (float)(x[1] * RAD2DEG);
        data.psi = (float)(x[2] * RAD2DEG);
        data.thetaMax = ThetaMax;

        data.thetaDot = (float)(v[0] * RAD2DEG);
        data.phiDot = (float)(v[1] * RAD2DEG);
        data.psiDot = (float)(v[2] * RAD2DEG);

        // Get local basis vectors
        Vector3 e1 = data.UHat;
        Vector3 e2 = data.E2Hat;
        Vector3 e3 = data.Y3Hat;

        // Compute angular velocity
        data.angularVelocity = data.thetaDot * e1 + data.phiDot * Vector3.up + data.psiDot * e3;
        // data.angularVelocity = data.thetaDot * e1 + data.phiDot * sin * e2 + (data.psiDot + data.phiDot * cos) * e3;

        // Compute angular momentum
        float sin = (float)Math.Sin(x[0]);
        float cos = (float)Math.Cos(x[0]);
        data.angularMomentum = data.I1 * data.thetaDot * e1 + data.I1 * data.phiDot * sin * e2 + data.I3 * (data.psiDot + data.phiDot * cos) * e3;

        // Compute torque
        data.torque = diskMass * gravity * diskOffset * sin * e1;

        // Compute Energy
        float energy = ComputeEnergy(e1, e2, sin, cos);
        if (data.initialEnergy == 0) data.initialEnergy = energy;
        data.totalEnergy = energy;
        data.energyRatio = data.totalEnergy / data.initialEnergy;

        // Check for runaway numerical errors
        CheckForInstability();
    }

    private void UpdateSimState(bool broadcast)
    {
        if (simState)
        {
            simState.data = data;
            if (broadcast) simState.BroadcastDataUpdated();
        }
    }

    public void SetGravity(float value)
    {
        gravity = value;
    }

    public void SetTheta0(float value)
    {
        // Debug.Log("TopSimulation > SetTheta");
        // theta0 = Mathf.Min(value, ThetaMax);
        theta0 = value;
        phi0 = (float)(x[1] * RAD2DEG);
        psi0 = (float)(x[2] * RAD2DEG);
        thetaDot0 = (float)(v[0] * RAD2DEG);
        phiDot0 = (float)(v[1] * RAD2DEG);
        psiDot0 = (float)(v[2] * RAD2DEG);
        Reset();
    }

    public void SetPhi0(float value)
    {
        // Debug.Log("TopSimulation > SetPhi");
        theta0 = (float)(x[0] * RAD2DEG);
        phi0 = value;
        psi0 = (float)(x[2] * RAD2DEG);
        thetaDot0 = (float)(v[0] * RAD2DEG);
        phiDot0 = (float)(v[1] * RAD2DEG);
        psiDot0 = (float)(v[2] * RAD2DEG);
        Reset();
    }

    public void SetPsi0(float value)
    {
        // Debug.Log("TopSimulation > SetPsi");
        theta0 = (float)(x[0] * RAD2DEG);
        phi0 = (float)(x[1] * RAD2DEG);
        psi0 = value;
        thetaDot0 = (float)(v[0] * RAD2DEG);
        phiDot0 = (float)(v[1] * RAD2DEG);
        psiDot0 = (float)(v[2] * RAD2DEG);
        Reset();
    }

    public void SetThetaDot0(float value)
    {
        // Debug.Log("TopSimulation > SetThetaDot");
        theta0 = (float)(x[0] * RAD2DEG);
        phi0 = (float)(x[1] * RAD2DEG);
        psi0 = (float)(x[2] * RAD2DEG);
        thetaDot0 = value;
        phiDot0 = (float)(v[1] * RAD2DEG);
        psiDot0 = (float)(v[2] * RAD2DEG);
        Reset();
    }

    public void SetPhiDot0(float value)
    {
        // Debug.Log("TopSimulation > SetPhiDot");
        theta0 = (float)(x[0] * RAD2DEG);
        phi0 = (float)(x[1] * RAD2DEG);
        psi0 = (float)(x[2] * RAD2DEG);
        thetaDot0 = (float)(v[0] * RAD2DEG);
        phiDot0 = value;
        psiDot0 = (float)(v[2] * RAD2DEG);
        Reset();
    }

    public void SetPsiDot0(float value)
    {
        // Debug.Log("TopSimulation > SetPsiDot");
        theta0 = (float)(x[0] * RAD2DEG);
        phi0 = (float)(x[1] * RAD2DEG);
        psi0 = (float)(x[2] * RAD2DEG);
        thetaDot0 = (float)(v[0] * RAD2DEG);
        phiDot0 = (float)(v[1] * RAD2DEG);
        psiDot0 = value;
        Reset();
    }

    public void SetDiskRadius(float value)
    {
        // Debug.Log("TopSimulation > SetDiskRadius");
        diskRadius = value;
        PropagateTopSettingChange();
    }

    public void SetDiskOffset(float value)
    {
        // Debug.Log("TopSimulation > SetDiskOffset");
        diskOffset = value;
        PropagateTopSettingChange();
    }

    public void SetDiskMass(float value)
    {
        // Debug.Log("TopSimulation > SetDiskMass");
        diskMass = value;
        data.initialEnergy = 0;  // Why do we need to do this for mass but not the others?
        PropagateTopSettingChange();
    }

    private void PropagateTopSettingChange()
    {
        // Propagate a change in settings through the relevant structures
        ComputeMomentOfInertia();
        ComputeAccelerations();
        ComputeMaxPolarAngle();
        UpdatePhysicalAppearance();
        UpdateData();
        UpdateSimState(true);
    }

    public override void TogglePlayPause()
    {
        base.TogglePlayPause();

        if (simState)
        {
            simState.data.simIsRunning = !IsPaused;
            // simState.BroadcastTogglePlayPause();
        }
    }

    public void Reset()
    {
        // Compute the top's moment of inertia components
        ComputeMomentOfInertia();
        // Initialize the ODE arrays
        InitializeODE();
        // Determine the max polar angle
        ComputeMaxPolarAngle();
        // Create a new data container and populate it with current values
        data = new TopData();
        UpdateData();
        // Synchronize the simulation state data and broadcast the update
        UpdateSimState(true);
        // Update the disk and rod game objects to match their settings values
        UpdatePhysicalAppearance();
        // Point the top in the right direction
        UpdateOrientation();

        Pause();
    }

    private void CheckForInstability()
    {
        if (data.energyRatio >= 2)
        {
            Pause();
            OnTopHasBecomeUnstable?.Invoke();
        }
    }
}

[Serializable]
public class TopData
{
    public bool simIsRunning;
    [Tooltip("In m / s^2")] public float gravity;

    [Header("Physical properties")]
    [Tooltip("In meters")] public float diskRadius;
    [Tooltip("In meters")] public float diskOffset;
    [Tooltip("In meters")] public float diskMass;
    [Tooltip("In kg * m^2")] public float I1;
    [Tooltip("In kg * m^2")] public float I3;

    [Header("Euler angles")]
    [Tooltip("In degrees")] public float theta;
    [Tooltip("In degrees")] public float phi;
    [Tooltip("In degrees")] public float psi;
    [Tooltip("In degrees")] public float thetaMax;

    [Header("Angular velocity")]
    [Tooltip("In deg / s")] public float thetaDot;
    [Tooltip("In deg / s")] public float phiDot;
    [Tooltip("In deg / s")] public float psiDot;
    [Tooltip("In deg / s")] public Vector3 angularVelocity;

    [Header("Dynamics")]
    [Tooltip("In kg * m^2 / s")] public Vector3 angularMomentum;
    [Tooltip("In kg * m^2 / s^2")] public Vector3 torque;

    [Header("Energy")]
    public float initialEnergy;
    public float totalEnergy;
    public float energyRatio;

    // The direction the rod is pointing
    public Vector3 Direction => Quaternion.Euler(0, -phi, theta) * Vector3.up;

    // Rotated basis vectors
    public Vector3 UHat => Quaternion.AngleAxis(-phi, Vector3.up) * Vector3.back;
    public Vector3 Y3Hat => Quaternion.AngleAxis(-theta, UHat) * Vector3.up;
    public Vector3 E2Hat => Vector3.Cross(UHat, Y3Hat).normalized;
}
