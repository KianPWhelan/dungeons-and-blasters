using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Keybinds")]
public class Keybinds : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<Binding> bindingSettings = new List<Binding>();

    public Dictionary<Actions, KeyCode> bindings;

    [System.Serializable]
    public struct Binding
    {
        public Actions action;
        public KeyCode key;
    }

    /// <summary>
    /// Updates the keybinds internally based on the public bindings list
    /// </summary>
    public void SetKeybinds()
    {
        bindings = new Dictionary<Actions, KeyCode>();

        foreach (Binding b in bindingSettings)
        {
            bindings.Add(b.action, b.key);
        }
    }

    public void OnBeforeSerialize()
    {
        //SetKeybinds();
    }

    public void OnAfterDeserialize()
    {
    }
}
