using UnityEngine;

public class Activity2Instructions : MonoBehaviour
{
    [SerializeField] private CanvasGroup step1;
    [SerializeField] private CanvasGroup step2;

    private void Start()
    {
        ShowStep(1);
    }

    public void ShowStep(int id)
    {
        if (id == 1)
        {
            // Show step 1
            ShowCanvasGroup(step1);
            HideCanvasGroup(step2);
        }
        else if (id == 2)
        {
            // Show step 2
            ShowCanvasGroup(step2);
            HideCanvasGroup(step1);
        }
    }

    private void ShowCanvasGroup(CanvasGroup cg)
    {
        if (cg)
        {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    private void HideCanvasGroup(CanvasGroup cg)
    {
        if (cg)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
}
