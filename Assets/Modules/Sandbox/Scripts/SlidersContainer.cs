using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SlidersContainer : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float enabledAlpha = 1;
    [SerializeField, Range(0, 1)] private float disabledAlpha = 1;

    private CanvasGroup canvasGroup;
    private List<Slider> sliders;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        sliders = new List<Slider>();

        foreach (Transform child in transform)
        {
            if (child.transform.TryGetComponent(out Slider slider))
            {
                sliders.Add(slider);
            }
        }
    }

    public void DisableSliders()
    {
        canvasGroup.alpha = disabledAlpha;
        canvasGroup.interactable = false;

        foreach (var slider in sliders)
        {
            slider.interactable = false;
            if (slider.gameObject.TryGetComponent(out CursorHoverUI cursorHover))
            {
                cursorHover.enabled = false;
            }
        }
    }

    public void EnableSliders()
    {
        canvasGroup.alpha = enabledAlpha;
        canvasGroup.interactable = true;

        foreach (var slider in sliders)
        {
            slider.interactable = true;
            if (slider.gameObject.TryGetComponent(out CursorHoverUI cursorHover))
            {
                cursorHover.enabled = true;
            }
        }
    }
}
