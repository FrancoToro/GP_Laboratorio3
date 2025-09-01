using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleLSystemTurtle))]
public class SimpleLSystemTurtleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Dibuja el inspector original
        DrawDefaultInspector();

        SimpleLSystemTurtle turtle = (SimpleLSystemTurtle)target;

        // Botón personalizado
        if (GUILayout.Button("Regenerar "))
        {
            turtle.RegenerateRandom();
        }
    }
}