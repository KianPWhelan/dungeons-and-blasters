using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UniqueIdGenerator))]
public class UniqueIdEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UniqueIdGenerator idGenerator = (UniqueIdGenerator)target;

        if (GUILayout.Button("Generate Unique ID"))
        {
            idGenerator.GenerateGUID();
        }
    }
}