using UnityEngine;

public class GraphManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TopSimulationState simState;
    [SerializeField] private DynamicGraph phiGraph;
    [SerializeField] private DynamicGraph thetaGraph;
    [SerializeField] private DynamicGraph phiDotGraph;
    [SerializeField] private DynamicGraph thetaDotGraph;
    [SerializeField] private DynamicGraph psiDotGraph;

    [Header("Graph Settings")]
    [SerializeField, Min(0)] private float graphDeltaTime = 0.1f;
    [SerializeField] private Color phiColor = Color.black;
    [SerializeField] private Color thetaColor = Color.black;
    [SerializeField] private Color phiDotColor = Color.black;
    [SerializeField] private Color thetaDotColor = Color.black;
    [SerializeField] private Color psiDotColor = Color.black;
    private float graphTime;
    private float elapsedTime;

    private void Start()
    {
        if (phiGraph) phiGraph.CreateLine(phiColor, "phi");
        if (thetaGraph) thetaGraph.CreateLine(thetaColor, "theta");
        if (phiDotGraph) phiDotGraph.CreateLine(phiDotColor, "phiDot");
        if (thetaDotGraph) thetaDotGraph.CreateLine(thetaDotColor, "thetaDot");
        if (psiDotGraph) psiDotGraph.CreateLine(psiDotColor, "psiDot");
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

            if (phiDotGraph)
            {
                Vector2 phiDot = new Vector2(elapsedTime, simState.data.phiDot);
                phiDotGraph.PlotPoint(0, phiDot);
            }

            if (thetaDotGraph)
            {
                Vector2 thetaDot = new Vector2(elapsedTime, simState.data.thetaDot);
                thetaDotGraph.PlotPoint(0, thetaDot);
            }

            if (psiDotGraph)
            {
                Vector2 psiDot = new Vector2(elapsedTime, simState.data.psiDot);
                psiDotGraph.PlotPoint(0, psiDot);
            }

            graphTime = 0;
        }
    }

    public void Reset()
    {
        if (phiGraph) phiGraph.Clear();
        if (thetaGraph) thetaGraph.Clear();
        if (phiDotGraph) phiDotGraph.Clear();
        if (thetaDotGraph) thetaDotGraph.Clear();
        if (psiDotGraph) psiDotGraph.Clear();

        graphTime = 0;
        elapsedTime = 0;
    }
}
