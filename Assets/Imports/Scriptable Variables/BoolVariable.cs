#region Description
// -----------------------------------------------------------------------------------------
// Self-contained asset-based bool variable
// -----------------------------------------------------------------------------------------
#endregion

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Bool")]
public class BoolVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("Initial value of variable to default to (This should generally not be changed during runtime)")]
    public bool initialValue;

    [Tooltip("Current value of variable in runtime (Methods should generally modify this)")]
    //[NonSerialized]
    public bool runtimeValue;

    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }

    public void OnBeforeSerialize()
    { }
}
