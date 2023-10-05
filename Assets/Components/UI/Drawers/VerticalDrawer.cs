using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VerticalDrawer : MonoBehaviour
{
    private RectTransform rectTransform;

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

        rectTransform.anchoredPosition = new Vector2(xShowing, yPosition);
    }

    public void SetVisibility(bool visible)
    {
        if (visible)
        {
            Hide(timeToHide);
        }
        else
        {
            Show(timeToShow);
        }
    }

    private void Hide(float lerpTime)
    {
        StopAllCoroutines();

        Vector2 targetPosition = new Vector2(xHidden, yPosition);
        StartCoroutine(LerpPosition(rectTransform, targetPosition, lerpTime, 2));
        if (container && fadeContents) StartCoroutine(LerpCanvasGroupAlpha(container, 0, timeToFadeOut));
        if (hiddenLabel && useHiddenLabel) StartCoroutine(LerpCanvasGroupAlpha(hiddenLabel, 1, lerpTime));
    }

    private void Show(float lerpTime)
    {
        StopAllCoroutines();

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
