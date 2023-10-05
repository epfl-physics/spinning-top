using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HorizontalDrawer : MonoBehaviour
{
    private RectTransform rectTransform;

    [Header("Position Settings")]
    [SerializeField] private float yHidden = -115;
    [SerializeField] private float yShowing = -20;
    [SerializeField] private float timeToHide = 0.5f;
    [SerializeField] private float timeToShow = 1f;

    [Header("Container Settings")]
    [SerializeField] private CanvasGroup container;
    [SerializeField] private bool fadeContents;
    [SerializeField] private float timeToFadeOut = 0.5f;
    [SerializeField] private float timeToFadeIn = 1f;

    private float xPosition;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        xPosition = rectTransform.anchoredPosition.x;
    }

    private void Start()
    {
        rectTransform.anchoredPosition = new Vector2(xPosition, yShowing);
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

        Vector2 targetPosition = new Vector2(xPosition, yHidden);
        StartCoroutine(LerpPosition(rectTransform, targetPosition, lerpTime, 2));
        if (container && fadeContents) StartCoroutine(LerpCanvasGroupAlpha(container, 0, timeToFadeOut));
    }

    private void Show(float lerpTime)
    {
        StopAllCoroutines();

        Vector2 targetPosition = new Vector2(xPosition, yShowing);
        StartCoroutine(LerpPosition(rectTransform, targetPosition, lerpTime, 2));
        if (container && fadeContents) StartCoroutine(LerpCanvasGroupAlpha(container, 1, timeToFadeIn));
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
