using UnityEngine;

public class TheoryManager : MonoBehaviour
{
    [Header("Sim References")]
    [SerializeField] private TopSimulation topSim;
    [SerializeField] private TopSimulation wheelSim;
    [SerializeField] private TopSimulationState simState;

    [Header("Camera")]
    [SerializeField] private TheoryCameraController cameraController;

    [Header("Control Buttons")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;

    private enum SimMode { Top, Wheel }
    [Header("Settings")]
    [SerializeField] private SimMode simMode = default;

    private float psiDotSign = 1;

    public void Start()
    {
        ResetSimulations();
        ResetCamera(1);
    }

    public void Play()
    {
        if (topSim)
        {
            if (simMode == SimMode.Top) topSim.Resume();
        }
        if (wheelSim)
        {
            if (simMode == SimMode.Wheel) wheelSim.Resume();
        }

        if (simState) simState.data.simIsRunning = true;
    }

    public void Pause()
    {
        if (topSim)
        {
            if (simMode == SimMode.Top) topSim.Pause();
        }
        if (wheelSim)
        {
            if (simMode == SimMode.Wheel) wheelSim.Pause();
        }
    }

    private void ResetUI()
    {
        if (playButton) playButton.SetActive(true);
        if (pauseButton) pauseButton.SetActive(false);
    }

    private void ResetSimulations()
    {
        // Put simulations in their initial states
        if (topSim)
        {
            if (simMode == SimMode.Top) topSim.Reset();
        }
        if (wheelSim)
        {
            if (simMode == SimMode.Wheel) wheelSim.Reset();
        }
    }

    private void ResetCamera(float moveTime)
    {
        if (cameraController)
        {
            if (simMode == SimMode.Top)
            {
                cameraController.MoveToTopPosition(moveTime);
            }
            else
            {
                cameraController.MoveToWheelPosition(moveTime);
            }
        }
    }

    public void Reset()
    {
        ResetUI();
        ResetSimulations();
        ResetCamera(0.7f);
    }

    public void ResetImmediately()
    {
        ResetUI();
        ResetSimulations();
        ResetCamera(0);
    }

    public void SetSimMode(bool isTop)
    {
        if (topSim) topSim.gameObject.SetActive(isTop);
        if (wheelSim) wheelSim.gameObject.SetActive(!isTop);

        simMode = isTop ? SimMode.Top : SimMode.Wheel;

        SetRotationDirection(psiDotSign == 1);

        Reset();
    }

    public void SetRotationDirection(bool isPositive)
    {
        psiDotSign = isPositive ? 1 : -1;

        if (topSim)
        {
            if (simMode == SimMode.Top) topSim.SetPsiDot0(psiDotSign * 3000);
        }
        if (wheelSim)
        {
            if (simMode == SimMode.Wheel)
            {
                wheelSim.SetPhiDot0(psiDotSign * 25);
                wheelSim.SetPsiDot0(psiDotSign * 3000);
            }
        }
    }

    public void SetRotationDirectionAndReset(bool isPositive)
    {
        ResetImmediately();
        SetRotationDirection(isPositive);
    }
}
