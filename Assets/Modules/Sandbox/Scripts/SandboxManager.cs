using System;
using UnityEngine;

public class SandboxManager : MonoBehaviour
{
    [Header("Top Simulation")]
    [SerializeField] private TopSimulation sim;
    [SerializeField] private TopSimulationState simState;
    [SerializeField] private InitialData initialData;

    [Header("Slider Groups")]
    [SerializeField] private SlidersContainer[] slidersContainers;

    [Header("Individual Sliders")]
    [SerializeField] private CustomSlider thetaSlider;
    [SerializeField] private CustomSlider phiSlider;
    [SerializeField] private CustomSlider psiSlider;
    [SerializeField] private CenteredSlider thetaDotSlider;
    [SerializeField] private CenteredSlider phiDotSlider;
    [SerializeField] private CenteredSlider psiDotSlider;
    [SerializeField] private CustomSlider diskRadiusSlider;
    [SerializeField] private CustomSlider diskOffsetSlider;
    [SerializeField] private CustomSlider diskMassSlider;

    [Header("Buttons")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;

    [Header("Panels")]
    [SerializeField] private GameObject oopsPanel;

    [Header("Object Outlines")]
    [SerializeField] private Transform rodOutline;

    private bool isUpdatingUI;

    private void OnEnable()
    {
        TopSimulation.OnTopHasBecomeUnstable += HandleTopHasBecomeUnstable;
    }

    private void OnDisable()
    {
        TopSimulation.OnTopHasBecomeUnstable -= HandleTopHasBecomeUnstable;
    }

    private void Start()
    {
        Reset();
    }

    public void Play()
    {
        foreach (SlidersContainer container in slidersContainers)
        {
            container.DisableSliders();
        }

        if (sim) sim.Resume();
    }

    public void Pause()
    {
        if (sim) sim.Pause();

        if (simState)
        {
            isUpdatingUI = true;
            if (thetaSlider) thetaSlider.value = simState.data.theta;
            if (phiSlider) phiSlider.value = simState.data.phi;
            if (psiSlider) psiSlider.value = simState.data.psi;
            if (thetaDotSlider) thetaDotSlider.value = simState.data.thetaDot;
            if (phiDotSlider) phiDotSlider.value = simState.data.phiDot;
            if (psiDotSlider) psiDotSlider.value = simState.data.psiDot;
            isUpdatingUI = false;
        }

        foreach (SlidersContainer container in slidersContainers)
        {
            container.EnableSliders();
        }
    }

    public void SetDiskRadius(float value)
    {
        // Debug.Log("SandboxManager > SetDiskRadius");
        if (isUpdatingUI || !sim) return;
        sim.SetDiskRadius(value);

        SetTheta0(thetaSlider.value);
    }

    public void SetDiskOffset(float value)
    {
        // Debug.Log("SandboxManager > SetDiskOffset");
        if (isUpdatingUI || !sim) return;
        sim.SetDiskOffset(value);

        SetTheta0(thetaSlider.value);
    }

    public void SetDiskMass(float value)
    {
        // Debug.Log("SandboxManager > SetDiskMass");
        if (isUpdatingUI || !sim) return;
        sim.SetDiskMass(value);
    }

    public void SetPhi0(float value)
    {
        if (isUpdatingUI || !sim) return;
        sim.SetPhi0(value);
    }

    public void SetPhiDot0(float value)
    {
        if (isUpdatingUI || !sim) return;
        sim.SetPhiDot0(value);
    }

    public void SetTheta0(float value)
    {
        if (isUpdatingUI || !sim) return;
        sim.SetTheta0(value);
        // ValidateThetaSlider();
    }

    public void ValidateThetaSlider()
    {
        isUpdatingUI = true;
        if (thetaSlider)
        {
            // Do not exceed the maximum allowed polar angle
            thetaSlider.value = Mathf.Min(thetaSlider.value, sim.ThetaMax);
            // Make sure the text display matches
            // thetaSlider.UpdateValue(theta);
        }
        isUpdatingUI = false;
    }

    public void SetThetaDot0(float value)
    {
        if (isUpdatingUI || !sim) return;
        sim.SetThetaDot0(value);
    }

    public void SetPsi0(float value)
    {
        if (isUpdatingUI || !sim) return;
        sim.SetPsi0(value);
    }

    public void SetPsiDot0(float value)
    {
        if (isUpdatingUI || !sim) return;
        sim.SetPsiDot0(value);
    }

    public void Reset()
    {
        // Set sliders to their initial values (without executing their onValueChanged callback)
        isUpdatingUI = true;
        if (diskRadiusSlider) diskRadiusSlider.value = initialData.diskRadius;
        if (diskOffsetSlider) diskOffsetSlider.value = initialData.diskOffset;
        if (diskMassSlider) diskMassSlider.value = initialData.diskMass;
        if (thetaSlider) thetaSlider.value = initialData.theta0;
        if (phiSlider) phiSlider.value = initialData.phi0;
        if (psiSlider) psiSlider.value = initialData.psi0;
        if (thetaDotSlider) thetaDotSlider.value = initialData.thetaDot0;
        if (phiDotSlider) phiDotSlider.value = initialData.phiDot0;
        if (psiDotSlider) psiDotSlider.value = initialData.psiDot0;
        isUpdatingUI = false;

        // Make sure sliders are interactable 
        foreach (SlidersContainer container in slidersContainers)
        {
            container.EnableSliders();
        }

        if (playButton) playButton.gameObject.SetActive(true);
        if (pauseButton) pauseButton.gameObject.SetActive(false);

        // Put top simulation in its initial state
        if (!sim) return;

        sim.SetGravity(initialData.gravity);
        sim.SetDiskRadius(initialData.diskRadius);
        sim.SetDiskOffset(initialData.diskOffset);
        sim.SetDiskMass(initialData.diskMass);
        sim.SetTheta0(initialData.theta0);
        sim.SetPhi0(initialData.phi0);
        sim.SetPsi0(initialData.psi0);
        sim.SetThetaDot0(initialData.thetaDot0);
        sim.SetPhiDot0(initialData.phiDot0);
        sim.SetPsiDot0(initialData.psiDot0);

        sim.Reset();
    }

    public void HandleTopHasBecomeUnstable()
    {
        if (oopsPanel) oopsPanel.SetActive(true);
    }
}

[Serializable]
public class InitialData
{
    public float gravity = 9.81f;

    [Header("Top Settings")]
    public float diskRadius = 1f;
    public float diskOffset = 2.4f;
    public float diskMass = 1f;

    [Header("Initial Conditions")]
    public float phi0 = 0;
    public float theta0 = 15;
    public float psi0 = 0;
    public float phiDot0 = 0;
    public float thetaDot0 = 0;
    public float psiDot0 = 3000;
}
