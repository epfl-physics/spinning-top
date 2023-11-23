using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform label;
    [SerializeField] private RectTransform toggles;

    [Header("Settings")]
    [SerializeField, Range(0, 1)] private float dividerPosition = 0.3f;

    public void SetDivider()
    {
        if (label)
        {
            label.anchorMin = Vector2.zero;
            label.anchorMax = new Vector2(dividerPosition, 1);
        }

        if (toggles)
        {
            toggles.anchorMin = new Vector2(dividerPosition, 0);
            toggles.anchorMax = Vector2.one;
        }
    }
}
