using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponDeprecated))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WeaponDeprecated weapon = (WeaponDeprecated)target;

        if (GUILayout.Button("Map()"))
        {
            weapon.Map();
        }
    }
}