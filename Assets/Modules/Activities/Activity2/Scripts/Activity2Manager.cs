using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Activity2Manager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TopSimulation sim;
    [SerializeField] private TopSimulationState simState;

    [Header("Instructions")]
    [SerializeField] private Activity2Instructions instructions;

    [Header("Vectors")]
    [SerializeField] private GameObject vectorPrefab;

    [Header("UI")]
    [SerializeField] private Button verifyButton;
    [SerializeField] private CanvasGroup winPanel;
    [SerializeField] private CanvasGroupFader tryAgainPanel;

    [Header("Feedback")]
    [SerializeField] private SoundEffect successBell;
    [SerializeField] private GameObject confetti;

    private const float torqueMagnitude = 1.2f;

    private Transform vectorContainer;
    private List<ClickableVector> vectors;
    private List<Vector3> initialComponents;

    private bool isFirstLoad = true;
    private Coroutine pulsateVectors;

    private void Awake()
    {
        vectorContainer = new GameObject("Vectors").transform;
        vectorContainer.SetParent(transform, false);

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

    private void Start()
    {
        Randomize();

        pulsateVectors = StartCoroutine(PulsateVectors());
    }

    private void LateUpdate()
    {
        if (!sim || !simState) return;

        // Update the torque vector
        if (!sim.IsPaused)
        {
            Vector3 torque = torqueMagnitude * simState.data.torque.normalized;
            // There should only be one vector remaining in the list if the sim is running
            vectors[0].SetComponents(torque);
        }
    }

    public void Randomize()
    {
        if (!sim) return;

        float phi = Random.Range(0, 359);
        float theta = Random.Range(45, 135);
        while (Mathf.Abs(90 - theta) < 5)
        {
            theta = Random.Range(45, 135);
        }
        if (isFirstLoad)
        {
            phi = 180;
            theta = 70;
            isFirstLoad = false;
        }
        float psiDot = new float[] { -3000, 3000 }[Random.Range(0, 2)];
        float mass = Random.Range(0.4f, 0.6f);

        // TODO this is not efficient, since each time a param is set the top redraws
        sim.SetPhi0(phi);
        sim.SetTheta0(theta);
        sim.SetPhiDot0(Mathf.Sign(psiDot) * 25);
        sim.SetPsiDot0(psiDot);
        sim.SetDiskMass(mass);

        ClearVectors();
        DrawOptionVectors();

        HideWinPanel();
        HideTryAgainPanel();
        SetButtonInteractable(verifyButton, false);
        ShowInstructionStep(1);
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

    private void ClearIncorrectVectors()
    {
        vectors.RemoveRange(1, vectors.Count - 1);

        for (int i = vectorContainer.childCount - 1; i > 0; i--)
        {
            DestroyImmediate(vectorContainer.GetChild(1).gameObject);
        }

        vectors[0].SetInteractable(false);
    }

    private void DrawOptionVectors()
    {
        if (!vectorPrefab || !simState) return;

        Vector3 torque = torqueMagnitude * simState.data.torque.normalized;

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

        SetButtonInteractable(verifyButton, true);

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

        if (noVectorSelected) SetButtonInteractable(verifyButton, false);
    }

    private void SetButtonInteractable(Button button, bool isInteractable)
    {
        if (!button) return;

        button.interactable = isInteractable;

        if (button.TryGetComponent(out CursorHoverUI cursorHover))
        {
            cursorHover.enabled = isInteractable;
        }
    }

    public void CheckAnswer()
    {
        if (vectors[0].IsSelected)
        {
            ClearIncorrectVectors();
            ShowWinPanel();
            vectors[0].SetLabelVisibility(true);
        }
        else
        {
            ShowTryAgainPanel();
        }
    }

    public void ShowWinPanel()
    {
        HideTryAgainPanel();

        if (winPanel)
        {
            winPanel.alpha = 1;
            winPanel.interactable = true;
            winPanel.blocksRaycasts = true;
        }

        if (successBell && TryGetComponent(out AudioSource audioSource))
        {
            successBell.Play(audioSource);
        }

        if (confetti) confetti.SetActive(true);
    }

    public void HideWinPanel()
    {
        if (winPanel)
        {
            winPanel.alpha = 0;
            winPanel.interactable = false;
            winPanel.blocksRaycasts = false;
        }
    }

    private void HideTryAgainPanel()
    {
        if (tryAgainPanel) tryAgainPanel.Hide();
    }

    private void ShowTryAgainPanel()
    {
        if (tryAgainPanel) tryAgainPanel.Show();
    }

    public void ShowInstructionStep(int id)
    {
        if (instructions) instructions.ShowStep(id);
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

    private void RestoreInitialVectors()
    {
        for (int i = 0; i < vectors.Count; i++)
        {
            ClickableVector vector = vectors[i];
            vector.SetComponents(initialComponents[i]);
        }
    }

    // public void HandleCameraMovementComplete(Vector3 position, Quaternion rotation)
    // {
    //     Debug.Log("Activity2Manager > HandleCameraMovementComplete");
    // }

    public void ResetCamera()
    {
        if (instructions) instructions.ResetCamera();
    }
}
