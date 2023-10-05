using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClassicEulerAngle)), CanEditMultipleObjects]
public class ClassicEulerAngleEditor : Editor
{
    private ClassicEulerAngle component;

    private SerializedProperty arcLR;
    private SerializedProperty sector;
    private SerializedProperty type;
    private SerializedProperty theta;
    private SerializedProperty phi;
    private SerializedProperty psi;
    private SerializedProperty radius;
    private SerializedProperty color;
    private SerializedProperty sectorAlpha;

    private void OnEnable()
    {
        component = (ClassicEulerAngle)target;

        arcLR = serializedObject.FindProperty("arcLR");
        sector = serializedObject.FindProperty("sector");
        type = serializedObject.FindProperty("type");
        theta = serializedObject.FindProperty("theta");
        phi = serializedObject.FindProperty("phi");
        psi = serializedObject.FindProperty("psi");
        radius = serializedObject.FindProperty("radius");
        color = serializedObject.FindProperty("color");
        sectorAlpha = serializedObject.FindProperty("sectorAlpha");
    }

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        serializedObject.Update();
        EditorGUILayout.PropertyField(arcLR);
        EditorGUILayout.PropertyField(sector);
        EditorGUILayout.PropertyField(type);
        EditorGUILayout.PropertyField(theta);
        EditorGUILayout.PropertyField(phi);
        EditorGUILayout.PropertyField(psi);
        EditorGUILayout.PropertyField(radius);
        // switch (type.enumValueIndex)
        // {
        //     case (int)EulerAngle.Type.Theta:
        //         EditorGUILayout.PropertyField(phi);
        //         break;
        //     case (int)EulerAngle.Type.Phi:
        //         break;
        //     case (int)EulerAngle.Type.Psi:
        //         EditorGUILayout.PropertyField(theta);
        //         EditorGUILayout.PropertyField(phi);
        //         break;
        //     default:
        //         break;
        // }

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
