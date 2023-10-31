using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TopSimulation))]
public class TopSimulationEditor : Editor
{
    private TopSimulation component;

    private void OnEnable()
    {
        component = (TopSimulation)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // if (GUI.changed)
        // {
        //     component.Initialize();
        //     component.Redraw();
        // }
    }
}
