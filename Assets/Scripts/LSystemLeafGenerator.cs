using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SimpleLSystemTurtle))]
[RequireComponent(typeof(LineRenderer))]
public class LSystemLeafGenerator : MonoBehaviour
{
    [Header("Prefab de la hoja")]
    [SerializeField] public GameObject leafPrefab;

    [Header("Opciones")]
    [SerializeField, Range(0.1f, 2f)] private float leafScale = 0.5f;
    [SerializeField] private bool randomRotation = true;

    private SimpleLSystemTurtle turtle;
    private LineRenderer lineRenderer;
    private readonly List<GameObject> spawnedLeaves = new List<GameObject>();

    private void Awake()
    {
        turtle = GetComponent<SimpleLSystemTurtle>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Borra todas las hojas instanciadas de forma segura.
    /// </summary>
    private void ClearLeaves()
    {
        for (int i = spawnedLeaves.Count - 1; i >= 0; i--)
        {
            var go = spawnedLeaves[i];
            if (!go) continue;

            if (Application.isPlaying)
                Destroy(go);
            else
                DestroyImmediate(go);
        }

        spawnedLeaves.Clear();
    }

    /// <summary>
    /// Genera hojas en la punta del L-System (último punto del LineRenderer).
    /// Botón disponible en el menú contextual del componente.
    /// </summary>
    [ContextMenu("Generar Hojas")]
    public void GenerateLeaves()
    {
        // Validaciones
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();

        if (leafPrefab == null)
        {
            Debug.LogWarning("Asigna un 'leafPrefab' en el inspector para generar hojas.", this);
            return;
        }

        int pointCount = lineRenderer.positionCount;
        if (pointCount < 2)
        {
            Debug.LogWarning("El LineRenderer no tiene suficientes puntos para colocar hojas.", this);
            return;
        }

        // Limpia hojas anteriores (FIX a tu línea 34)
        ClearLeaves();

        // Coloca la hoja en la punta final
        Vector3 lastPoint = lineRenderer.GetPosition(pointCount - 1);
        SpawnLeaf(lastPoint);
    }

    /// <summary>
    /// Instancia una hoja en la posición indicada y la registra.
    /// </summary>
    private void SpawnLeaf(Vector3 position)
    {
        var leaf = Instantiate(leafPrefab, position, Quaternion.identity, transform);
        leaf.transform.localScale = Vector3.one * leafScale;

        if (randomRotation)
            leaf.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));

        spawnedLeaves.Add(leaf);
    }
}
