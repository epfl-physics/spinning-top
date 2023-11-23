using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TogglePanel)), CanEditMultipleObjects]
public class TogglePanelEditor : Editor
{
    private TogglePanel component;

    private void OnEnable()
    {
        component = (TogglePanel)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUI.changed)
        {
            component.SetDivider();
        }
    }
}
