using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity2Step1 : MonoBehaviour
{
    [Header("Vectors")]
    [SerializeField] private GameObject vectorPrefab;

    [Header("UI")]
    [SerializeField] private ActivityVerifyButton verifyButton;

    private Transform vectorContainer;
    private List<ClickableVector> vectors;
    private List<Vector3> initialComponents;

    private Coroutine pulsateVectors;

    public static event System.Action OnAnswerCorrect;
    public static event System.Action OnAnswerIncorrect;

    private void Awake()
    {
        vectorContainer = new GameObject("Clickable Vectors").transform;
        vectors = new List<ClickableVector>();
        initialComponents = new List<Vector3>();
    }

    private void OnEnable()
    {
        ClickableVector.OnSelected += HandleVectorSelected;
        ClickableVector.OnDeselected += HandleVectorDeselected;
        // CameraController.OnCameraMovementComplete += HandleCameraMovementComplete;
    }

    private void OnDisable()
    {
        ClickableVector.OnSelected -= HandleVectorSelected;
        ClickableVector.OnDeselected -= HandleVectorDeselected;
        // CameraController.OnCameraMovementComplete -= HandleCameraMovementComplete;
    }

    public void Load(TopSimulationState simState, float torqueMagnitude)
    {
        DeactivateVerifyButton();

        // Create six clickable vectors : 1 true torque and 5 incorrect options
        Vector3 torque = torqueMagnitude * simState.data.torque.normalized;
        DrawOptionVectors(torque);

        // Visual cue for the user to click on a vector
        pulsateVectors = StartCoroutine(PulsateVectors());
    }

    public void HandleVectorSelected(ClickableVector selectedVector)
    {
        if (!vectors.Contains(selectedVector)) return;

        // Debug.Log(selectedVector.name + " selected");
        foreach (ClickableVector vector in vectors)
        {
            if (vector != selectedVector)
            {
                if (vector.IsSelected) vector.OnClick();
            }
        }

        ActivateVerifyButton();

        if (pulsateVectors != null)
        {
            StopCoroutine(pulsateVectors);
            RestoreInitialVectors();
            pulsateVectors = null;
        }
    }

    public void HandleVectorDeselected(ClickableVector deselectedVector)
    {
        if (!vectors.Contains(deselectedVector)) return;

        bool noVectorSelected = true;

        // Debug.Log(deselectedVector.name + " deselected");
        foreach (ClickableVector vector in vectors)
        {
            if (vector.IsSelected)
            {
                noVectorSelected = false;
                break;
            }
        }

        if (noVectorSelected) DeactivateVerifyButton();
    }

    private IEnumerator PulsateVectors()
    {
        yield return new WaitForSeconds(2);

        float time = 0;
        while (time < 3f)
        {
            time += Time.deltaTime;
            for (int i = 0; i < vectors.Count; i++)
            {
                ClickableVector vector = vectors[i];
                Vector3 components = initialComponents[i];
                vector.SetComponents((1 + 0.05f * Mathf.Sin(2 * Mathf.PI * time)) * components);
            }
            yield return null;
        }

        RestoreInitialVectors();
        pulsateVectors = null;
    }

    private void ClearVectors()
    {
        vectors.Clear();

        // Destroy any current vectors
        for (int i = vectorContainer.childCount; i > 0; i--)
        {
            DestroyImmediate(vectorContainer.GetChild(0).gameObject);
        }
    }

    // private void ClearIncorrectVectors()
    // {
    //     vectors.RemoveRange(1, vectors.Count - 1);

    //     for (int i = vectorContainer.childCount - 1; i > 0; i--)
    //     {
    //         DestroyImmediate(vectorContainer.GetChild(1).gameObject);
    //     }

    //     vectors[0].SetInteractable(false);
    // }

    private void DrawOptionVectors(Vector3 torque)
    {
        // Make room for a new set of vectors to pulsate
        initialComponents.Clear();

        if (!vectorPrefab) return;

        // Create new vectors
        ClickableVector vectorTrue = Instantiate(vectorPrefab, vectorContainer).GetComponent<ClickableVector>();
        vectorTrue.name = "Truth";
        vectorTrue.SetComponents(torque);
        vectors.Add(vectorTrue);
        initialComponents.Add(vectorTrue.Components);

        ClickableVector vector1 = Instantiate(vectorPrefab, vectorContainer).GetComponent<ClickableVector>();
        vector1.name = "Vector1";
        vector1.SetComponents(-torque);
        vectors.Add(vector1);
        initialComponents.Add(vector1.Components);

        ClickableVector vector2 = Instantiate(vectorPrefab, vectorContainer).GetComponent<ClickableVector>();
        vector2.name = "Vector2";
        vector2.SetComponents(Quaternion.Euler(0, -90, 0) * torque);
        vectors.Add(vector2);
        initialComponents.Add(vector2.Components);

        ClickableVector vector3 = Instantiate(vectorPrefab, vectorContainer).GetComponent<ClickableVector>();
        vector3.name = "Vector3";
        vector3.SetComponents(-vector2.Components);
        vectors.Add(vector3);
        initialComponents.Add(vector3.Components);

        ClickableVector vector4 = Instantiate(vectorPrefab, vectorContainer).GetComponent<ClickableVector>();
        vector4.name = "Vector4";
        vector4.SetComponents(torque.magnitude * Vector3.up);
        vectors.Add(vector4);
        initialComponents.Add(vector4.Components);

        ClickableVector vector5 = Instantiate(vectorPrefab, vectorContainer).GetComponent<ClickableVector>();
        vector5.name = "Vector5";
        vector5.SetComponents(torque.magnitude * Vector3.down);
        vectors.Add(vector5);
        initialComponents.Add(vector5.Components);
    }

    private void RestoreInitialVectors()
    {
        for (int i = 0; i < vectors.Count; i++)
        {
            ClickableVector vector = vectors[i];
            vector.SetComponents(initialComponents[i]);
        }
    }

    private void ActivateVerifyButton()
    {
        if (!verifyButton) return;

        verifyButton.Enable();

        if (verifyButton.TryGetComponent(out CursorHoverUI cursorHover))
        {
            cursorHover.enabled = true;
        }
    }

    private void DeactivateVerifyButton()
    {
        if (!verifyButton) return;

        verifyButton.Disable();

        if (verifyButton.TryGetComponent(out CursorHoverUI cursorHover))
        {
            cursorHover.enabled = false;
        }
    }

    public void CheckAnswer()
    {
        if (vectors[0].IsSelected)
        {
            ClearVectors();
            OnAnswerCorrect?.Invoke();
        }
        else
        {
            OnAnswerIncorrect?.Invoke();
        }
    }
}
