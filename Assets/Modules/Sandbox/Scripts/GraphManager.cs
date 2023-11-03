using UnityEngine;

public class GraphManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TopSimulationState simState;
    [SerializeField] private DynamicGraph phiGraph;
    [SerializeField] private DynamicGraph thetaGraph;

    [Header("Graph Settings")]
    [SerializeField, Min(0)] private float graphDeltaTime = 0.1f;
    [SerializeField] private Color phiColor = Color.black;
    [SerializeField] private Color thetaColor = Color.black;
    private float graphTime;
    private float elapsedTime;

    private void Start()
    {
        if (phiGraph) phiGraph.CreateLine(phiColor, "phi");
        if (thetaGraph) thetaGraph.CreateLine(thetaColor, "theta");
    }

    private void Update()
    {
        if (simState.SimIsRunning) elapsedTime += Time.deltaTime;

        graphTime += Time.deltaTime;

        if (graphTime >= graphDeltaTime)
        {
            if (phiGraph)
            {
                Vector2 phi = new Vector2(elapsedTime, simState.data.phi);
                phiGraph.PlotPoint(0, phi);
            }

            if (thetaGraph)
            {
                Vector2 theta = new Vector2(elapsedTime, simState.data.theta);
                thetaGraph.PlotPoint(0, theta);
            }

            graphTime = 0;
        }
    }

    public void Reset()
    {
        if (phiGraph) phiGraph.Clear();
        if (thetaGraph) thetaGraph.Clear();

        graphTime = 0;
        elapsedTime = 0;
    }
}
