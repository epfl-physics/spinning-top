using UnityEngine;

public class CubeSimulation : Simulation
{
    public Transform cube;
    public bool isRotating;
    public CubeSimulationState simState;

    private void OnEnable()
    {
        if (simState) cube.rotation = simState.rotation;
    }

    private void Update()
    {
        if (!cube || IsPaused) return;

        if (isRotating)
        {
            float deltaAngle = 20 * Time.deltaTime;
            cube.RotateAround(cube.position, (Vector3.up + Vector3.right).normalized, deltaAngle);
        }

        if (simState) simState.rotation = cube.rotation;
    }

    public void SetCubeSize(float value)
    {
        if (cube) cube.localScale = value * Vector3.one;
    }
}
