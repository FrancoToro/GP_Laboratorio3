using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Randomiza los parámetros del L-System cada cierto intervalo de tiempo.
/// </summary>
[RequireComponent(typeof(LSystemTurtle))]
public class LSystemTurtleRandomizer : MonoBehaviour
{
    [Header("Intervalo de randomización")]
    [SerializeField, Range(0f, 20f)] 
    private float interval = 5f; // Cada cuántos segundos se randomiza

    [Header("Ángulo")]
    [SerializeField, Range(-80f, 80f)] 
    private float minAngle = -10f;

    [SerializeField, Range(-80f, 80f)] 
    private float maxAngle = 10f;

    [Header("Longitud de línea")]
    [SerializeField, Range(0.1f, 10f)] 
    private float minLineLength = 1f;

    [SerializeField, Range(0.1f, 10f)] 
    private float maxLineLength = 5f;

    [Header("Generaciones del L-System")]
    [SerializeField, Range(1, 5)] 
    private int minGenerations = 1;

    [SerializeField, Range(1, 5)] 
    private int maxGenerations = 3;

    [Header("Opciones visuales")]
    [SerializeField] 
    private bool generateRandomMaterial = false;

    private float timer = 0f;
    private LSystemTurtle lSystemTurtle;

    private void Awake()
    {
        // Obtiene referencia al componente principal
        lSystemTurtle = GetComponent<LSystemTurtle>();

        // Configura material inicial si corresponde
        lSystemTurtle.generateRandomMaterial = generateRandomMaterial;

        // Primera randomización inmediata
        ApplyRandomization();
    }

    private void Update()
    {
        // Si el intervalo es 0 o menor, randomiza cada frame
        if (interval <= 0f)
        {
            ApplyRandomization();
            return;
        }

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            ApplyRandomization();
            timer = 0f;
        }
    }

    /// <summary>
    /// Aplica valores aleatorios a los parámetros del L-System y lo regenera.
    /// </summary>
    private void ApplyRandomization()
    {
        // Asegura que los rangos sean válidos (min <= max)
        float angle = Random.Range(Mathf.Min(minAngle, maxAngle), Mathf.Max(minAngle, maxAngle));
        float length = Random.Range(Mathf.Min(minLineLength, maxLineLength), Mathf.Max(minLineLength, maxLineLength));
        int generations = Random.Range(Mathf.Min(minGenerations, maxGenerations), Mathf.Max(minGenerations, maxGenerations) + 1);

        // Aplica los valores al L-System
        lSystemTurtle.angle = angle;
        lSystemTurtle.lineLength = length;
        lSystemTurtle.numberOfGenerations = generations;

        // Regenera el sistema con limpieza
        lSystemTurtle.Generate(clean: true);
    }
}
