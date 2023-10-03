using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CircularSector))]
public class CircularSectorEditor : Editor
{
    private CircularSector component;

    private SerializedProperty angle;
    private SerializedProperty radius;
    private SerializedProperty normal;
    private SerializedProperty e1;
    private SerializedProperty color;

    private void OnEnable()
    {
        component = (CircularSector)target;

        angle = serializedObject.FindProperty("angle");
        radius = serializedObject.FindProperty("radius");
        normal = serializedObject.FindProperty("normal");
        e1 = serializedObject.FindProperty("e1");
        color = serializedObject.FindProperty("color");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(angle);
        EditorGUILayout.PropertyField(radius);
        EditorGUILayout.PropertyField(normal);
        EditorGUILayout.PropertyField(e1);
        EditorGUILayout.PropertyField(color);
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            component.Redraw();
        }
    }
}
