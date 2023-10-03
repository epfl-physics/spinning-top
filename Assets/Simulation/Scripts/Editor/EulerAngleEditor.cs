using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EulerAngle))]
public class EulerAngleEditor : Editor
{
    private EulerAngle component;

    private SerializedProperty referenceLR;
    private SerializedProperty directionLR;
    private SerializedProperty arcLR;
    private SerializedProperty sector;
    private SerializedProperty type;
    private SerializedProperty value;
    private SerializedProperty theta;
    private SerializedProperty phi;
    private SerializedProperty axisLength;
    private SerializedProperty radius;
    private SerializedProperty color;
    private SerializedProperty sectorAlpha;

    private void OnEnable()
    {
        component = (EulerAngle)target;

        referenceLR = serializedObject.FindProperty("referenceLR");
        directionLR = serializedObject.FindProperty("directionLR");
        arcLR = serializedObject.FindProperty("arcLR");
        sector = serializedObject.FindProperty("sector");
        type = serializedObject.FindProperty("type");
        value = serializedObject.FindProperty("value");
        theta = serializedObject.FindProperty("theta");
        phi = serializedObject.FindProperty("phi");
        axisLength = serializedObject.FindProperty("axisLength");
        radius = serializedObject.FindProperty("radius");
        color = serializedObject.FindProperty("color");
        sectorAlpha = serializedObject.FindProperty("sectorAlpha");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(referenceLR);
        EditorGUILayout.PropertyField(directionLR);
        EditorGUILayout.PropertyField(arcLR);
        EditorGUILayout.PropertyField(sector);
        EditorGUILayout.PropertyField(type);
        EditorGUILayout.PropertyField(value);
        switch (type.enumValueIndex)
        {
            case (int)EulerAngle.Type.Theta:
                EditorGUILayout.PropertyField(phi);
                EditorGUILayout.PropertyField(axisLength);
                break;
            case (int)EulerAngle.Type.Phi:
                EditorGUILayout.PropertyField(axisLength);
                break;
            case (int)EulerAngle.Type.Psi:
                EditorGUILayout.PropertyField(theta);
                EditorGUILayout.PropertyField(phi);
                EditorGUILayout.PropertyField(axisLength);
                EditorGUILayout.PropertyField(radius);
                break;
            default:
                break;
        }

        EditorGUILayout.PropertyField(color);
        if (sector != null)
        {
            EditorGUILayout.PropertyField(sectorAlpha);
        }
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            component.Redraw();
        }
    }
}
