using System;
using UnityEngine;
using System.Collections.Generic;
using LAB3.Core.Utilities;

/// <summary>
/// Implementación de una tortuga para interpretar y dibujar un L-System.
/// Usa un LineRenderer para generar las líneas a partir de la cadena generada.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LSystemTurtle : MonoBehaviour 
{
    [Header("Configuración del L-System")]
    [SerializeField] public LSystem lSystem;          // Objeto que contiene las reglas y la sentencia generada.
    [SerializeField, Range(1, 100)] public float lineLength = 5.0f;
    [SerializeField, Range(-100, 100)] public float angle = 10f;
    [SerializeField, Range(1, 5)] public int numberOfGenerations = 1;

    [Header("Opciones Visuales")]
    [SerializeField] public bool generateRandomMaterial = false;

    private LineRenderer lineRenderer;                // LineRenderer base (plantilla).
    private int currentLine = 0;                      // Índice de la línea actual.
    private LSystemState state = new LSystemState();  // Estado actual de la tortuga.
    private Stack<LSystemState> savedState = new();   // Pila de estados guardados (para [ y ]).
    private List<GameObject> lines = new();           // Lista de segmentos generados.
    private Material randomMaterial;                  // Material aleatorio (si está activado).

    private void Start() 
    {
        // Si se requiere, crea un material aleatorio con color único
        if (generateRandomMaterial)
        {
            randomMaterial = MaterialUtils.CreateMaterialWithRandomColor($"{gameObject.name}_material");
        }

        Generate();
    }

    /// <summary>
    /// Genera el L-System y dibuja las líneas correspondientes.
    /// </summary>
    /// <param name="clean">Si es true, limpia el sistema existente antes de generar.</param>
    public void Generate(bool clean = false)
    {
        // Validaciones iniciales
        if (lSystem == null)
        {
            Debug.LogError("Debes asignar un L-System.");
            enabled = false;
            return;
        }
        if (lSystem.RuleCount == 0)
        {
            Debug.LogError("El L-System debe tener al menos una regla definida.");
            enabled = false;
            return;
        }

        // Guarda la sentencia original del L-System
        lSystem.SaveOriginalSentence();

        // Limpia las líneas si es necesario
        if (clean) CleanExistingLSystem();

        // Prepara el LineRenderer base
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        // Ejecuta las generaciones solicitadas
        for (int i = 0; i < numberOfGenerations; i++)
        {
            savedState.Push(state.Clone());
            lSystem.Generate();
            state = savedState.Pop();
        }

        // Dibuja el resultado final
        DrawLines();
    }

    /// <summary>
    /// Crea un nuevo segmento de línea en la posición actual de la tortuga.
    /// </summary>
    private void Line()
    {
        var lineGo = new GameObject($"Line_{currentLine}");
        lineGo.transform.parent = transform;
        lines.Add(lineGo);

        LineRenderer newLine = SetupLine(lineGo);

        // Punto inicial
        newLine.SetPosition(0, GetWorldPosition());

        // Avanza y obtiene nueva posición
        MoveForward();

        // Punto final
        newLine.SetPosition(1, GetWorldPosition());

        currentLine++;
    }

    /// <summary>
    /// Limpia todas las líneas y resetea el L-System al estado original.
    /// </summary>
    private void CleanExistingLSystem() 
    {
        lSystem.RestoreToOriginalSentence();
        savedState.Clear();

        foreach (GameObject line in lines)
        {
#if UNITY_EDITOR
            DestroyImmediate(line, true);
#else
            Destroy(line);
#endif
        }

        lines.Clear();
        currentLine = 0;
    }

    /// <summary>
    /// Avanza la tortuga según el ángulo actual.
    /// </summary>
    private void MoveForward()
    {
        // Convierte el ángulo a radianes
        float rad = state.angle * Mathf.Deg2Rad;

        // Calcula desplazamiento
        state.x += Mathf.Sin(rad) * state.size;
        state.y += Mathf.Cos(rad) * state.size;
    }

    /// <summary>
    /// Obtiene la posición en el mundo de la tortuga considerando el offset del objeto.
    /// </summary>
    private Vector3 GetWorldPosition()
    {
        return new Vector3(
            state.x + transform.position.x, 
            state.y + transform.position.y, 
            transform.position.z
        );
    }

    /// <summary>
    /// Interpreta la sentencia generada por el L-System y dibuja las líneas.
    /// </summary>
    private void DrawLines()
    {
        // Estado inicial
        state = new LSystemState
        {
            x = 0,
            y = 0,
            size = lineLength,
            angle = 0
        };

        string sentence = lSystem.GeneratedSentence;

        foreach (char c in sentence)
        {
            switch (c)
            {
                case 'F': Line(); break;             // Dibuja una línea
                case 'G': MoveForward(); break;      // Solo traslada sin dibujar
                case '+': state.angle += angle; break; // Gira a la derecha
                case '-': state.angle -= angle; break; // Gira a la izquierda
                case '[': savedState.Push(state.Clone()); break; // Guarda estado
                case ']': state = savedState.Pop(); break;       // Restaura estado
            }
        }
    }

    /// <summary>
    /// Configura un nuevo LineRenderer copiando las propiedades del base.
    /// </summary>
    private LineRenderer SetupLine(GameObject lineGo)
    {
        var newLine = lineGo.AddComponent<LineRenderer>();
        newLine.useWorldSpace = true;
        newLine.positionCount = 2;
        newLine.tag = "Line";

        // Usa material aleatorio si está activado
        newLine.material = generateRandomMaterial ? randomMaterial : lineRenderer.material;

        // Copia estilos del LineRenderer base
        newLine.startColor = lineRenderer.startColor;
        newLine.endColor = lineRenderer.endColor;
        newLine.startWidth = lineRenderer.startWidth;
        newLine.endWidth = lineRenderer.endWidth;
        newLine.numCapVertices = 5;

        return newLine;
    }
}
