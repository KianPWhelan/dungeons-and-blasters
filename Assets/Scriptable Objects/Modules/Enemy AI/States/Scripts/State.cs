using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class State : ScriptableObject
{
    public abstract string name
    {
        get;
    }

    public abstract void Tick();

    public abstract void OnStateEnter();

    public abstract void OnStateExit();
}
