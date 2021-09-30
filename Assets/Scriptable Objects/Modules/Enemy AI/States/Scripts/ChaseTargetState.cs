using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/ChaseTargetState")]
public class ChaseTargetState : State
{
    public override string name
    {
        get
        {
            return "ChaseTargetState";
        }
    }

    public override void OnStateEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        throw new System.NotImplementedException();
    }

    public override void OnStateExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        throw new System.NotImplementedException();
    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        throw new System.NotImplementedException();
    }
}
