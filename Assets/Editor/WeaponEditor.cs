using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Weapon weapon = (Weapon)target;

        if (GUILayout.Button("Map()"))
        {
            weapon.Map();
        }
    }
}