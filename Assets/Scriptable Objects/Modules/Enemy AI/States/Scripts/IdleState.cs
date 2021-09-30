using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/IdleState")]
public class IdleState : State
{
    public override string name
    {
        get
        {
            return "IdleState";
        }
    }

    public override void OnEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        Debug.Log("Enemy " + self.GetInstanceID() + " has entered " + name);
    }

    public override void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {

    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        
    }
}
