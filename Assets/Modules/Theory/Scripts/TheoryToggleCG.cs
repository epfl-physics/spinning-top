using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TheoryToggleCG : MonoBehaviour
{
    [SerializeField] private bool startActive = true;
    [SerializeField] private float inactiveAlpha = 0.3f;

    private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        SetVisibility(startActive);
    }

    public void SetVisibility(bool isVisible)
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = isVisible ? 1 : inactiveAlpha;
        canvasGroup.blocksRaycasts = isVisible;
        canvasGroup.interactable = isVisible;
    }
}
