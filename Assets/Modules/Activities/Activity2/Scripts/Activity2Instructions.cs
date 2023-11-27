using UnityEngine;

public class Activity2Instructions : MonoBehaviour
{
    [Header("Instruction Steps")]
    [SerializeField] private Activity2Step1 step1;
    [SerializeField] private Activity2Step2 step2;
    [SerializeField] private Activity2Step3 step3;

    // Canvas group references for each step
    private CanvasGroup step1CG;
    private CanvasGroup step2CG;
    private CanvasGroup step3CG;

    [Header("Labels")]
    [SerializeField] private GameObject principalAxesOrigin;

    [Header("Vectors")]
    [SerializeField] private Vector weightVector;
    [SerializeField] private Vector angularMomentumVector;
    [SerializeField] private Vector torqueVector;
    [SerializeField] private float torqueMagnitude = 1.2f;

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;

    // Camera
    private Vector3 position1;
    private Vector3 rotation1;

    private int currentStepIndex;

    private void Awake()
    {
        step1.TryGetComponent(out step1CG);
        step2.TryGetComponent(out step2CG);
        step3.TryGetComponent(out step3CG);

        HideCanvasGroup(step1CG);
        HideCanvasGroup(step2CG);
        HideCanvasGroup(step3CG);

        if (cameraController)
        {
            position1 = cameraController.targetPosition;
            rotation1 = cameraController.targetRotation;
        }
    }

    public void LoadStep1(TopSimulationState simState)
    {
        if (!step1) return;

        step1.Load(simState, torqueMagnitude);

        if (weightVector)
        {
            weightVector.transform.position = simState.data.diskOffset * simState.data.Direction;
            weightVector.components = simState.data.gravity * simState.data.diskMass * Vector3.down;
            weightVector.Redraw();
        }

        if (angularMomentumVector) angularMomentumVector.gameObject.SetActive(false);
        if (torqueVector) torqueVector.gameObject.SetActive(false);

        if (principalAxesOrigin && simState)
        {
            Vector3 positionG = (simState.data.diskOffset - 0.4f) * simState.data.Direction + 0.4f * simState.data.E2Hat;
            principalAxesOrigin.transform.position = positionG;
        }

        ShowCanvasGroup(step1CG);
        HideCanvasGroup(step2CG);
        HideCanvasGroup(step3CG);

        currentStepIndex = 1;
    }

    private void LoadStep2(TopSimulationState simState)
    {
        if (!step2) return;

        step2.Load(simState);

        ShowCanvasGroup(step2CG);
        HideCanvasGroup(step1CG);
        HideCanvasGroup(step3CG);

        if (angularMomentumVector)
        {
            angularMomentumVector.components = 1.5f * torqueMagnitude * simState.data.angularMomentum.normalized;
            angularMomentumVector.gameObject.SetActive(true);
        }

        if (torqueVector)
        {
            torqueVector.components = torqueMagnitude * simState.data.torque.normalized;
            torqueVector.gameObject.SetActive(true);
        }

        currentStepIndex = 2;
    }

    private void LoadStep3(TopSimulation sim, TopSimulationState simState)
    {
        if (!step3) return;

        step3.Load(simState, weightVector, angularMomentumVector, torqueVector, torqueMagnitude);

        ShowCanvasGroup(step3CG);
        HideCanvasGroup(step1CG);
        HideCanvasGroup(step2CG);

        if (sim) sim.Resume();

        currentStepIndex = 3;
    }

    public void LoadNextStep(TopSimulation sim, TopSimulationState simState)
    {
        if (currentStepIndex == 1)
            LoadStep2(simState);
        else if (currentStepIndex == 2)
            LoadStep3(sim, simState);
    }

    private void ShowCanvasGroup(CanvasGroup cg)
    {
        if (cg)
        {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    private void HideCanvasGroup(CanvasGroup cg)
    {
        if (cg)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    // private void ZoomOut(float distance)
    // {
    //     Transform cameraTransform = Camera.main.transform;
    //     Vector3 position = distance * cameraTransform.position.normalized;
    //     MoveCameraTo(position, cameraTransform.rotation.eulerAngles);
    // }

    private void MoveCameraTo(Vector3 position, Vector3 rotation)
    {
        if (!cameraController) return;

        cameraController.targetPosition = position;
        cameraController.targetRotation = rotation;
        cameraController.TriggerCameraMovement();
    }

    public void ResetCamera()
    {
        // Debug.Log("Activity2Instructions > ResetCamera");
        MoveCameraTo(position1, rotation1);
    }
}
