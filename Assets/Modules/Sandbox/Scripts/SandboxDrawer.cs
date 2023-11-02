using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SandboxDrawer : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private bool startClosed;

    [Header("Button")]
    [SerializeField] private ArrowButton arrowButton;

    [Header("Position Settings")]
    [SerializeField] private float xHidden = -115;
    [SerializeField] private float xShowing = -20;
    [SerializeField] private float timeToHide = 0.5f;
    [SerializeField] private float timeToShow = 1f;

    [Header("Container Settings")]
    [SerializeField] private CanvasGroup container;
    [SerializeField] private bool fadeContents;
    [SerializeField] private float timeToFadeOut = 0.5f;
    [SerializeField] private float timeToFadeIn = 1f;

    [Header("Hidden Label")]
    [SerializeField] private CanvasGroup hiddenLabel;
    [SerializeField] private bool useHiddenLabel;

    private float yPosition;
    private bool isOpen;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        yPosition = rectTransform.anchoredPosition.y;
    }

    private void Start()
    {
        if (hiddenLabel)
        {
            hiddenLabel.alpha = 0;
            hiddenLabel.blocksRaycasts = false;
        }

        if (startClosed)
            Close(0);
        else
            Open(0);
    }

    public void ToggleState()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    public void SetState(bool open)
    {
        if (open)
            Open();
        else
            Close();
    }

    public void Close()
    {
        Close(timeToHide);
    }

    public void Open()
    {
        Open(timeToShow);
    }

    private void Close(float lerpTime)
    {
        StopAllCoroutines();

        isOpen = false;
        if (arrowButton) arrowButton.SetSprite(isOpen);

        Vector2 targetPosition = new Vector2(xHidden, yPosition);
        StartCoroutine(LerpPosition(rectTransform, targetPosition, lerpTime, 2));
        if (container && fadeContents) StartCoroutine(LerpCanvasGroupAlpha(container, 0, timeToFadeOut));
        if (hiddenLabel && useHiddenLabel) StartCoroutine(LerpCanvasGroupAlpha(hiddenLabel, 1, lerpTime));
    }

    private void Open(float lerpTime)
    {
        StopAllCoroutines();

        isOpen = true;
        if (arrowButton) arrowButton.SetSprite(isOpen);

        Vector2 targetPosition = new Vector2(xShowing, yPosition);
        StartCoroutine(LerpPosition(rectTransform, targetPosition, lerpTime, 2));
        if (container && fadeContents) StartCoroutine(LerpCanvasGroupAlpha(container, 1, timeToFadeIn));
        if (hiddenLabel && useHiddenLabel) StartCoroutine(LerpCanvasGroupAlpha(hiddenLabel, 0, 0.6f * lerpTime));
    }

    private IEnumerator LerpPosition(RectTransform rectTransform,
                                     Vector2 targetPosition,
                                     float lerpTime,
                                     float easeFactor = 0)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;

        float time = 0;
        while (time < lerpTime)
        {
            time += Time.unscaledDeltaTime;
            float t = EaseOut(time / lerpTime, 1 + easeFactor);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    private float EaseOut(float t, float a)
    {
        return 1 - Mathf.Pow(1 - t, a);
    }

    private IEnumerator LerpCanvasGroupAlpha(CanvasGroup cg,
                                             float targetAlpha,
                                             float lerpTime,
                                             float easeFactor = 0)
    {
        float startAlpha = cg.alpha;
        cg.blocksRaycasts = targetAlpha == 1;

        float time = 0;
        while (time < lerpTime)
        {
            time += Time.unscaledDeltaTime;
            float t = EaseOut(time / lerpTime, 1 + easeFactor);
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        cg.alpha = targetAlpha;
    }
}

