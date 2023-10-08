using UnityEditor;

[CustomEditor(typeof(CameraOrbit))]
public class CameraOrbitEditor : Editor
{
    SerializedProperty orbitTarget;
    SerializedProperty canOrbit;
    SerializedProperty canZoom;
    SerializedProperty orbitSpeed;
    SerializedProperty minPolarAngle;
    SerializedProperty maxPolarAngle;
    SerializedProperty clampAzimuthalAngle;
    SerializedProperty minAzimuthalAngle;
    SerializedProperty maxAzimuthalAngle;
    SerializedProperty minDistance;
    SerializedProperty maxDistance;
    SerializedProperty zoomSlider;

    private void OnEnable()
    {
        orbitTarget = serializedObject.FindProperty("target");
        canOrbit = serializedObject.FindProperty("canOrbit");
        canZoom = serializedObject.FindProperty("canZoom");
        orbitSpeed = serializedObject.FindProperty("orbitSpeed");
        minPolarAngle = serializedObject.FindProperty("minPolarAngle");
        maxPolarAngle = serializedObject.FindProperty("maxPolarAngle");
        clampAzimuthalAngle = serializedObject.FindProperty("clampAzimuthalAngle");
        minAzimuthalAngle = serializedObject.FindProperty("minAzimuthalAngle");
        maxAzimuthalAngle = serializedObject.FindProperty("maxAzimuthalAngle");
        minDistance = serializedObject.FindProperty("minDistance");
        maxDistance = serializedObject.FindProperty("maxDistance");
        zoomSlider = serializedObject.FindProperty("zoomSlider");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(orbitTarget);

        if (orbitTarget.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(canOrbit);
            if (canOrbit.boolValue)
            {
                EditorGUILayout.PropertyField(orbitSpeed);
                EditorGUILayout.PropertyField(minPolarAngle);
                EditorGUILayout.PropertyField(maxPolarAngle);
                EditorGUILayout.PropertyField(clampAzimuthalAngle);

                if (clampAzimuthalAngle.boolValue)
                {
                    EditorGUILayout.PropertyField(minAzimuthalAngle);
                    EditorGUILayout.PropertyField(maxAzimuthalAngle);
                }
            }

            EditorGUILayout.PropertyField(canZoom);
            if (canZoom.boolValue)
            {
                EditorGUILayout.PropertyField(minDistance);
                EditorGUILayout.PropertyField(maxDistance);
                EditorGUILayout.PropertyField(zoomSlider);
            }
        }
        else
        {
            canOrbit.boolValue = false;
            canZoom.boolValue = false;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
