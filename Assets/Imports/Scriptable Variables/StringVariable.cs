#region Description
// -----------------------------------------------------------------------------------------
// Self-contained asset-based string variable
// -----------------------------------------------------------------------------------------
#endregion

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/String")]
public class StringVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("Initial value of variable to default to (This should generally not be changed during runtime)")]
    [TextArea(minLines: 0, maxLines: 10)]
    public string initialValue;

    [Tooltip("Current value of variable in runtime (Methods should generally modify this)")]
    [NonSerialized]
    public string runtimeValue;

    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }

    public void OnBeforeSerialize()
    { }
}
