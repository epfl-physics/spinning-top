using UnityEngine;

public class Activity2Step3 : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField] private GameObject principalAxesOrigin;

    private bool isLoaded;
    private TopSimulationState simState;
    private Vector weightVector;
    private Vector angularMomentumVector;
    private Vector torqueVector;
    private float torqueMagnitude;

    public void Load(TopSimulationState simState, Vector weightVector, Vector angularMomentumVector, Vector torqueVector, float torqueMagnitude)
    {
        this.simState = simState;
        this.weightVector = weightVector;
        this.angularMomentumVector = angularMomentumVector;
        this.torqueVector = torqueVector;
        this.torqueMagnitude = torqueMagnitude;
        isLoaded = true;
    }

    public void Unload()
    {
        isLoaded = false;
        simState = null;
        weightVector = null;
        angularMomentumVector = null;
        torqueVector = null;
        torqueMagnitude = 0;
    }

    private void LateUpdate()
    {
        if (!isLoaded) return;

        if (weightVector)
        {
            weightVector.transform.position = simState.data.diskOffset * simState.data.Direction;
            weightVector.components = simState.data.gravity * simState.data.diskMass * Vector3.down;
            weightVector.Redraw();
        }

        if (angularMomentumVector)
        {
            angularMomentumVector.components = 1.5f * torqueMagnitude * simState.data.angularMomentum.normalized;
            angularMomentumVector.Redraw();
        }

        if (torqueVector)
        {
            torqueVector.components = torqueMagnitude * simState.data.torque.normalized;
            torqueVector.Redraw();
        }

        PlacePrincipalAxisOrigin();
    }

    public void PlacePrincipalAxisOrigin()
    {
        if (principalAxesOrigin && simState)
        {
            principalAxesOrigin.transform.position = (simState.data.diskOffset - 0.4f) * simState.data.Direction + 0.4f * simState.data.E2Hat;
        }
    }
}
