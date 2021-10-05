#region Description
// -----------------------------------------------------------------------------------------
// Self-contained asset-based float variable
// -----------------------------------------------------------------------------------------
#endregion

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Float")]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("Initial value of variable to default to (This should generally not be changed during runtime)")]
    public float initialValue;

    [Tooltip("Current value of variable in runtime (Methods should generally modify this)")]
    // [NonSerialized]
    public float runtimeValue;

    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }

    public void OnBeforeSerialize()
    { }
}
