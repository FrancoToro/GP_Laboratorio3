using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class SimpleLSystemTurtle : MonoBehaviour
{
    [Header("Configuración del L-System")]
    [SerializeField] public string axiom = "F";             // Sentencia inicial
    [SerializeField] private string rule = "F+F-F";          // Regla simple para F

    [Header("Generaciones")]
    [SerializeField, Range(1, 5)] public int minGenerations = 1;
    [SerializeField, Range(1, 5)] public int maxGenerations = 3;

    [Header("Ángulo")]
    [SerializeField, Range(0f, 90f)] public float minAngle = 15f;
    [SerializeField, Range(0f, 90f)] public float maxAngle = 45f;

    [Header("líneas")]
    [SerializeField, Range(0.1f, 5f)] public float lineLength = 1f;
    [SerializeField, Range(0.01f, 0.5f)] public float lineWidth = 0.05f;

    [Header("Material")]
    [SerializeField] private Material lineMaterial; 
    
    private string currentSentence;
    private LineRenderer lineRenderer;
    private float chosenAngle;   // Ángulo aleatorio elegido
    private int chosenGenerations; // Nº de generaciones aleatorio

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.material = lineMaterial;
        // Elegir valores aleatorios
        chosenAngle = Random.Range(minAngle, maxAngle);
        chosenGenerations = Random.Range(minGenerations, maxGenerations + 1);

        // Genera
        GenerateSentence();
        //dibuja
        DrawSentence();
    }
    //permite generar otra variante por consola
    [ContextMenu("Regenerar L-System Aleatorio")]
    public void RegenerateRandom()
    {
        // Elegir valores aleatorios
        chosenAngle = Random.Range(minAngle, maxAngle);
        chosenGenerations = Random.Range(minGenerations, maxGenerations + 1);

        // Generar y dibujar
        GenerateSentence();
        DrawSentence();
    }


    // Genera la sentencia.
    private void GenerateSentence()
    {
        currentSentence = axiom;

        for (int i = 0; i < chosenGenerations; i++)
        {
            string newSentence = "";

            foreach (char c in currentSentence)
            {
                if (c == 'F')
                    newSentence += rule; // Aplica la regla a F
                else
                    newSentence += c.ToString(); // Mantiene otros símbolos (+ y -)
            }

            currentSentence = newSentence;
        }
    }
    
    // Dibuja la sentencia usando un único LineRenderer.

    private void DrawSentence()
    {
        Vector3 position = Vector3.zero;
        float currentAngle = 0f;

        List<Vector3> points = new List<Vector3>();
        points.Add(position);

        foreach (char c in currentSentence)
        {
            switch (c)
            {
                case 'F':
                    // Avanza en la dirección actual
                    position += Quaternion.Euler(0, 0, currentAngle) * Vector3.up * lineLength;
                    points.Add(position);
                    break;

                case '+':
                    currentAngle += chosenAngle; // gira a la derecha
                    break;

                case '-':
                    currentAngle -= chosenAngle; // gira a la izquierda
                    break;
            }
        }

        // Configura el LineRenderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }
}
