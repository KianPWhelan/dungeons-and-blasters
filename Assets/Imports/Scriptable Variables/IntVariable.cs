#region Description
// -----------------------------------------------------------------------------------------
// Self-contained asset-based int variable
// -----------------------------------------------------------------------------------------
#endregion

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Int")]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("Initial value of variable to default to (This should generally not be changed during runtime)")]
    public int initialValue;

    [Tooltip("Current value of variable in runtime (Methods should generally modify this)")]
    [NonSerialized]
    public int runtimeValue;

    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }

    public void OnBeforeSerialize()
    { }
}
