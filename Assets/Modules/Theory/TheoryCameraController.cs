using System.Collections;
using UnityEngine;

public class TheoryCameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CameraOrbit cameraOrbit;
    [SerializeField] private CameraState cameraState;
    private new Camera camera;

    [Header("Top")]
    [SerializeField] private Vector3 topPosition = new Vector3(0, 5, -10);
    [SerializeField] private Vector3 topRotation = new Vector3(20, 0, 0);
    [SerializeField] private Transform topTarget;

    [Header("Wheel")]
    [SerializeField] private Vector3 wheelPosition = new Vector3(0, 5, -10);
    [SerializeField] private Vector3 wheelRotation = new Vector3(20, 0, 0);
    [SerializeField] private Transform wheelTarget;

    private Coroutine cameraMotion;

    public static event System.Action OnStartCameraMotion;
    public static event System.Action OnStopCameraMotion;

    private void Awake()
    {
        // Place the camera based on state parameters
        if (cameraTransform)
        {
            camera = cameraTransform.GetComponent<Camera>();

            if (cameraState)
            {
                cameraTransform.position = cameraState.position;
                cameraTransform.rotation = cameraState.rotation;
                cameraTransform.localScale = cameraState.scale;
            }
        }

        if (cameraOrbit) cameraOrbit.target = null;
    }

    private IEnumerator MoveTo(Vector3 targetPosition, Vector3 targetRotation, float moveTime)
    {
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;
        Quaternion finalRotation = Quaternion.Euler(targetRotation);
        float time = 0;

        if (moveTime > 0) OnStartCameraMotion?.Invoke();

        while (time < moveTime)
        {
            time += Time.deltaTime;
            float t = time / moveTime;
            t = t * t * (3f - 2f * t);
            cameraTransform.position = Vector3.Slerp(startPosition, targetPosition, t);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, finalRotation, t);
            UpdateCameraState();
            yield return null;
        }

        cameraTransform.position = targetPosition;
        cameraTransform.rotation = finalRotation;
        UpdateCameraState();

        if (cameraOrbit)
        {
            cameraOrbit.Initialize();
            cameraOrbit.canOrbit = true;
        }

        // Alert other scripts (e.g. SlideControllers) that the camera has finished moving
        if (moveTime > 0) OnStopCameraMotion?.Invoke();

        cameraMotion = null;
    }

    private void UpdateCameraState()
    {
        if (cameraState) cameraState.SetState(camera);
    }

    public void MoveToTopPosition(float moveTime)
    {
        if (cameraMotion != null)
        {
            StopCoroutine(cameraMotion);
            cameraMotion = null;
        }

        if (cameraOrbit)
        {
            cameraOrbit.target = topTarget;
            cameraOrbit.minPolarAngle = -45;
            cameraOrbit.maxPolarAngle = 89.9f;
            cameraOrbit.canOrbit = false;
        }

        cameraMotion = StartCoroutine(MoveTo(topPosition, topRotation, moveTime));
    }

    public void MoveToWheelPosition(float moveTime)
    {
        if (cameraMotion != null)
        {
            StopCoroutine(cameraMotion);
            cameraMotion = null;
        }

        if (cameraOrbit)
        {
            cameraOrbit.target = wheelTarget;
            cameraOrbit.minPolarAngle = -85f;
            cameraOrbit.maxPolarAngle = 89.9f;
            cameraOrbit.canOrbit = false;
        }

        cameraMotion = StartCoroutine(MoveTo(wheelPosition, wheelRotation, moveTime));
    }
}
