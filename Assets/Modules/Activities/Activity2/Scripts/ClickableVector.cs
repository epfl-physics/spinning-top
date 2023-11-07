using UnityEngine;

public class ClickableVector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Vector vector;
    [SerializeField] private Transform clickZone;

    [Header("Settings")]
    [SerializeField] private Vector3 components = Vector3.zero;
    [SerializeField] private Color selectedColor = Color.black;
    [SerializeField] private float defaultWidth = 0.2f;

    private Color originalColor;
    private bool selected;

    public bool IsSelected => selected;
    public Vector3 Components => components;

    public static event System.Action<ClickableVector> OnSelected;
    public static event System.Action<ClickableVector> OnDeselected;

    private void Awake()
    {
        if (vector) originalColor = vector.color;
    }

    public void SetComponents(Vector3 components)
    {
        this.components = components;
        Redraw();
    }

    public void Redraw()
    {
        float trueWidth = defaultWidth;

        if (vector)
        {
            vector.components = components;
            vector.Redraw();
            trueWidth = 2 * vector.lineWidth;
        }

        if (clickZone)
        {
            clickZone.localScale = new Vector3(components.magnitude, trueWidth, trueWidth);
            clickZone.localPosition = 0.5f * components;
            clickZone.right = components.normalized;
        }
    }

    public void OnClick()
    {
        Color newColor = selected ? originalColor : selectedColor;

        if (vector)
        {
            vector.color = newColor;
            vector.SetColor();
        }

        selected = !selected;

        if (selected)
            OnSelected?.Invoke(this);
        else
            OnDeselected?.Invoke(this);
    }

    public void SetInteractable(bool interactable)
    {
        if (clickZone.TryGetComponent(out ClickableObject clickableObject))
        {
            clickableObject.Interactable = interactable;
        }
    }
}
