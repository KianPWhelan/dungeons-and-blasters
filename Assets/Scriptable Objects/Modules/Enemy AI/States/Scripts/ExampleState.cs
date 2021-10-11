using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExampleState : State
{
    public override string name
    {
        get
        {
            return "ExampleState";
        }
    }

    public override void OnEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }

    public override void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }
}
