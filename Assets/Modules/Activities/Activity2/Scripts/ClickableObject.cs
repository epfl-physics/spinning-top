using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] private CustomCursor customCursor;

    private ClickableVector parentScript;
    private bool interactable = true;

    public bool Interactable { get => interactable; set => interactable = value; }

    private void Awake()
    {
        parentScript = GetComponentInParent<ClickableVector>();

        if (parentScript == null)
        {
            Debug.LogWarning("No ClickableVector component found on the parent!");
        }
    }

    private void OnDisable()
    {
        RestoreDefaultCursor();
    }

    private void OnMouseDown()
    {
        if (!interactable) return;

        parentScript?.OnClick();
    }

    private void OnMouseEnter()
    {
        if (!interactable) return;

        // Display the cursor while hovering
        if (customCursor) Cursor.SetCursor(customCursor.texture, customCursor.hotspot, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        if (!interactable) return;

        RestoreDefaultCursor();
    }

    private void RestoreDefaultCursor()
    {
        // Restore the default cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
