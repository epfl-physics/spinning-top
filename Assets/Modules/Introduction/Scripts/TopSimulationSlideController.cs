using Slides;
using UnityEngine;

public class TopSimulationSlideController : SimulationSlideController
{
    [SerializeField] private bool startPaused;

    public override void InitializeSlide()
    {
        if (simulation && startPaused) simulation.Pause();
    }
}
