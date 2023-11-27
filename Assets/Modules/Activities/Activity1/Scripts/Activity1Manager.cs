using UnityEngine;

public class Activity1Manager : MonoBehaviour
{
    [SerializeField] private Transform ghostTop;

    [Header("Euler Angles")]
    [SerializeField] private float thetaTrue;
    [SerializeField] private float phiTrue;
    [SerializeField] private float psiTrue;

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;

    [Header("Basis Vectors")]
    [SerializeField] private GameObject x3;

    [Header("UI")]
    [SerializeField] private Activity1Sliders sliders;
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CustomSlider zoomSlider;

    [Header("Feedback")]
    [SerializeField] private SoundEffect successBell;
    [SerializeField] private GameObject confetti;
    [SerializeField] private bool volumeIsOn = true;

    private const float tol = 2f; // in degrees
    private bool isFirstLoad = true;

    private void OnEnable()
    {
        ClassicEulerAngles.OnValuesChanged += CheckAnswer;
    }

    private void OnDisable()
    {
        ClassicEulerAngles.OnValuesChanged -= CheckAnswer;
    }

    private void Awake()
    {
        // Randomize();
        HideWinPanel();
    }

    public void Randomize()
    {
        // Pick random Euler angles
        thetaTrue = Random.Range(5, 175);
        if (isFirstLoad)
        {
            // Avoid starting too close to the vertical axis
            thetaTrue = Random.Range(30, 150);
            isFirstLoad = false;
        }
        phiTrue = Random.Range(2, 358);
        psiTrue = Random.Range(2, 358);

        // Initial relative basis vectors
        Vector3 x1 = Vector3.back;
        Vector3 x3 = Vector3.up;

        // Rotate the basis
        Quaternion rotation = Quaternion.identity;
        Vector3 y1 = Quaternion.AngleAxis(-phiTrue, Vector3.up) * x1;
        rotation = Quaternion.AngleAxis(-phiTrue, Vector3.up) * rotation;
        Vector3 y3 = Quaternion.AngleAxis(-thetaTrue, y1) * x3;
        rotation = Quaternion.AngleAxis(-thetaTrue, y1) * rotation;
        rotation = Quaternion.AngleAxis(-psiTrue, y3) * rotation;

        // Update the reference object's orientation
        if (ghostTop) ghostTop.rotation = rotation;
    }

    public void CheckAnswer(float theta, float phi, float psi)
    {
        bool answerIsCorrect;
        answerIsCorrect = Mathf.Abs(theta - thetaTrue) < tol;
        answerIsCorrect &= Mathf.Abs(phi - phiTrue) < tol;
        answerIsCorrect &= Mathf.Abs(psi - psiTrue) < tol;

        if (answerIsCorrect) ShowWinPanel();

        // Hide x3 if theta is zero
        if (x3) x3.SetActive(theta > 1);
    }

    public void Reset()
    {
        HideWinPanel();
        Randomize();

        if (cameraController) cameraController.TriggerCameraMovement();

        if (sliders)
        {
            sliders.Reset();
            sliders.SetInteractable(true);
        }

        if (confetti) confetti.SetActive(false);
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

    public void ShowWinPanel()
    {
        if (winPanel)
        {
            winPanel.alpha = 1;
            winPanel.interactable = true;
            winPanel.blocksRaycasts = true;
        }

        if (sliders) sliders.SetInteractable(false);

        if (volumeIsOn && successBell && TryGetComponent(out AudioSource audioSource))
        {
            successBell.Play(audioSource);
        }

        if (confetti) confetti.SetActive(true);
    }

    public void ToggleVolume(bool isOn)
    {
        volumeIsOn = isOn;
    }
}
