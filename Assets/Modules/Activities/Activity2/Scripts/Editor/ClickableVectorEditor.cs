using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClickableVector))]
public class ClickableVectorEditor : Editor
{
    private ClickableVector component;

    private void OnEnable()
    {
        component = (ClickableVector)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUI.changed)
        {
            component.Redraw();
        }
    }
}
