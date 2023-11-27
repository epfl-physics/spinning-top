using UnityEngine;

public class Activity2Step2 : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private OptionSet optionSet;

    private int correctIndex = -1;

    public static event System.Action OnAnswerCorrect;
    public static event System.Action OnAnswerIncorrect;

    public void Load(TopSimulationState simState)
    {
        correctIndex = simState.data.phiDot > 0 ? 0 : 1;
    }

    public void CheckAnswer()
    {
        if (!optionSet) return;

        if (optionSet.SelectedIndex == correctIndex)
        {
            optionSet.Reset();
            OnAnswerCorrect?.Invoke();
        }
        else
        {
            OnAnswerIncorrect?.Invoke();
        }
    }
}
