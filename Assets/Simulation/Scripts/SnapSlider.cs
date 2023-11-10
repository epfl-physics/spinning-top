using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SnapSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float increment = 1f;
    [SerializeField] private List<float> excludedValues;

    private void Reset()
    {
        // Ensure that there's a reference to the Slider component when adding to the GameObject
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(SnapValue);
        SnapValue(slider.value);
    }

    private void SnapValue(float value)
    {
        // Calculate the nearest increment value
        float newValue = Mathf.Round(value / increment) * increment;

        // Check if the value is in the excluded list, and if so, move to the next valid increment.
        while (excludedValues.Contains(newValue))
        {
            newValue += increment;
        }

        // Update the slider value without triggering the callback
        slider.onValueChanged.RemoveListener(SnapValue);
        slider.value = newValue;
        slider.onValueChanged.AddListener(SnapValue);
    }

    public void SetIncrement(float newIncrement)
    {
        increment = newIncrement;
    }
}
