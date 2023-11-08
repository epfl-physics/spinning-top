using UnityEngine;

public class Activity2Instructions : MonoBehaviour
{
    [SerializeField] private CanvasGroup step1;
    [SerializeField] private CanvasGroup step2;

    [Header("Step 2 camera")]
    [SerializeField] private CameraController cameraController;
    // [SerializeField] private Vector3 position2 = 10 * Vector3.back;
    // [SerializeField] private Vector3 rotation2 = default;
    // [SerializeField] private float cameraDistance2 = 15;

    private Vector3 position1;
    private Vector3 rotation1;

    private void Start()
    {
        ShowStep(1);

        if (cameraController)
        {
            position1 = cameraController.targetPosition;
            rotation1 = cameraController.targetRotation;
        }
    }

    public void ShowStep(int id)
    {
        if (id == 1)
        {
            // Show step 1
            ShowCanvasGroup(step1);
            HideCanvasGroup(step2);
        }
        else if (id == 2)
        {
            // Show step 2
            ShowCanvasGroup(step2);
            HideCanvasGroup(step1);
            // ZoomOut(cameraDistance2);
        }
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
