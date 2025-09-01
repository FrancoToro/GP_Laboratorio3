using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleLSystemTurtle2))]
public class SimpleLSystemTurtleEditor2 : Editor
{
    public override void OnInspectorGUI()
    {
        // Dibuja el inspector original
        DrawDefaultInspector();

        SimpleLSystemTurtle turtle = (SimpleLSystemTurtle)target;

        // Botón personalizado
        if (GUILayout.Button("Regenerar mejorado "))
        {
            turtle.RegenerateRandom();
        }
    }
}