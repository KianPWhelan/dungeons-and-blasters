using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JSONTools))]
public class JSONToolsEditor : Editor
{   public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        JSONTools jSONTools = (JSONTools)target;

        if (GUILayout.Button("Save Prefab Data To JSON Text"))
        {
            jSONTools.SaveGameObjectsToJSONTextFile();
        }

        if(GUILayout.Button("Load Prefab Data from JSON Text"))
        {
            jSONTools.LoadEnemyPrefabBalanceDataFromTextFile();
        }
    }
}
