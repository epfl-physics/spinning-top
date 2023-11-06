using UnityEngine;

[CreateAssetMenu(menuName = "Top Simulation/Top Simulation State", fileName = "Top Simulation State", order = 60)]
public class TopSimulationState : ScriptableObject
{
    public TopData data;

    public float ThetaMax => data.thetaMax;
    public bool SimIsRunning => data.simIsRunning;

    public static event System.Action OnUpdateData;
    // public static event System.Action OnTogglePlayPause;

    public void BroadcastDataUpdated()
    {
        OnUpdateData?.Invoke();
    }

    // public void BroadcastTogglePlayPause()
    // {
    //     OnTogglePlayPause?.Invoke();
    // }
}
