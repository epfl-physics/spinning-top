using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    [SerializeField] private float timeVisible = 2;
    [SerializeField] private float timeToFade = 0.5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        StartCoroutine(WaitAndFade());
    }

    public void Hide()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator WaitAndFade()
    {
        yield return new WaitForSeconds(timeVisible);

        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < timeToFade)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, time / timeToFade);
            yield return null;
        }

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
