using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public abstract class State : ScriptableObject
{
    public abstract string name
    {
        get;
    }

    public abstract void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement);

    public abstract void OnEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement);

    public abstract void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement);
}
