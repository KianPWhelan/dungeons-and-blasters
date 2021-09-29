using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomAI))]
public class CustomAIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CustomAI customAI = (CustomAI)target;

        if (GUILayout.Button("BuildStateMachine()"))
        {
            customAI.BuildStateMachine();
        }
    }
}
