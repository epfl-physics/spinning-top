using UnityEngine;

public class Activity2Manager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TopSimulation sim;
    [SerializeField] private TopSimulationState simState;

    [Header("Instructions")]
    [SerializeField] private Activity2Instructions instructions;

    [Header("Feedback")]
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CanvasGroupFader tryAgainPanel;
    [SerializeField] private SoundEffect successBell;
    [SerializeField] private SoundEffect nopeSound;
    [SerializeField] private GameObject confetti;
    [SerializeField] private bool volumeIsOn = true;

    private bool isFirstLoad = true;
    private float signPsiDot = -1;

    private void OnEnable()
    {
        Activity2Step1.OnAnswerCorrect += ShowWinPanel;
        Activity2Step1.OnAnswerIncorrect += ShowTryAgainPanel;

        Activity2Step2.OnAnswerCorrect += ShowWinPanel;
        Activity2Step2.OnAnswerIncorrect += ShowTryAgainPanel;

        // CameraController.OnCameraMovementComplete += HandleCameraMovementComplete;
    }

    private void OnDisable()
    {
        Activity2Step1.OnAnswerCorrect -= ShowWinPanel;
        Activity2Step1.OnAnswerIncorrect -= ShowTryAgainPanel;

        Activity2Step2.OnAnswerCorrect -= ShowWinPanel;
        Activity2Step2.OnAnswerIncorrect -= ShowTryAgainPanel;

        // CameraController.OnCameraMovementComplete -= HandleCameraMovementComplete;
    }

    private void Start()
    {
        Reset();
    }

    public void InitializeSimulation()
    {
        HideWinPanel();
        HideTryAgainPanel();

        if (!sim) return;

        // Make sure the simulation isn't running
        sim.Pause();

        // Choose an azimuthal angle
        float phi = isFirstLoad ? 180 : Random.Range(0, 4) * 90;
        if (isFirstLoad) isFirstLoad = false;

        // Set the other two Euler angles
        float theta = 90;
        float psiDot = signPsiDot * 3000;
        signPsiDot *= -1;  // Switch sign for next time

        // TODO this is not efficient, since each time a param is set the top redraws
        sim.SetPhi0(phi);
        sim.SetTheta0(theta);
        sim.SetPhiDot0(Mathf.Sign(psiDot) * 25);
        sim.SetPsiDot0(psiDot);
    }

    public void ShowWinPanel()
    {
        HideTryAgainPanel();

        if (winPanel)
        {
            winPanel.alpha = 1;
            winPanel.interactable = true;
            winPanel.blocksRaycasts = true;
        }

        if (volumeIsOn && successBell && TryGetComponent(out AudioSource audioSource))
        {
            successBell.Play(audioSource);
        }

        if (confetti) confetti.SetActive(true);
    }

    public void HideWinPanel()
    {
        if (winPanel)
        {
            winPanel.alpha = 0;
            winPanel.interactable = false;
            winPanel.blocksRaycasts = false;
        }
    }

    private void HideTryAgainPanel()
    {
        if (tryAgainPanel) tryAgainPanel.Hide();
    }

    private void ShowTryAgainPanel()
    {
        if (tryAgainPanel) tryAgainPanel.Show();

        if (volumeIsOn && nopeSound && TryGetComponent(out AudioSource audioSource))
        {
            nopeSound.Play(audioSource);
        }
    }

    public void LoadNextStep()
    {
        HideWinPanel();

        if (instructions) instructions.LoadNextStep(sim, simState);
    }

    // public void HandleCameraMovementComplete(Vector3 position, Quaternion rotation)
    // {
    //     Debug.Log("Activity2Manager > HandleCameraMovementComplete");
    // }

    public void ResetCamera()
    {
        if (instructions) instructions.ResetCamera();
    }

    public void Reset()
    {
        // Put the paused simulation in a random initial state
        InitializeSimulation();

        if (instructions) instructions.LoadStep1(simState);
    }

    public void ToggleVolume(bool isOn)
    {
        volumeIsOn = isOn;
    }
}
