#region Description
// -----------------------------------------------------------------------------------------
// Custom editor that allows GameEvent objects to be triggered through the inspector
// -----------------------------------------------------------------------------------------
#endregion

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameEvent myGameEvent = (GameEvent)target;

        if(GUILayout.Button("Raise()"))
        {
            myGameEvent.Raise();
        }
    }
}
