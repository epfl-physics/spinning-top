using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TheoryCanvasGroupController : MonoBehaviour
{
    // [SerializeField] private float inactiveAlpha = 0.5f;

    private CanvasGroup canvasGroup;
    // private float initialAlpha;
    private bool interactable;
    private bool blockRaycasts;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // initialAlpha = canvasGroup.alpha;
        interactable = canvasGroup.interactable;
        blockRaycasts = canvasGroup.blocksRaycasts;
    }

    private void OnEnable()
    {
        TheoryCameraController.OnStartCameraMotion += HandleCameraMotionStarted;
        TheoryCameraController.OnStopCameraMotion += HandleCameraMotionStopped;
    }

    private void OnDisable()
    {
        TheoryCameraController.OnStartCameraMotion -= HandleCameraMotionStarted;
        TheoryCameraController.OnStopCameraMotion -= HandleCameraMotionStopped;
    }

    public void HandleCameraMotionStarted()
    {
        // Debug.Log(transform.name + " HandleCameraMovementStarted");
        // canvasGroup.alpha = inactiveAlpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void HandleCameraMotionStopped()
    {
        // Debug.Log(transform.name + " HandleCameraMovementStopped");
        // canvasGroup.alpha = initialAlpha;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = blockRaycasts;
    }
}
