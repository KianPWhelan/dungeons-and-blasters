using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/ChaseTargetState")]
public class ChaseTargetState : State
{
    public override void OnEnter(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        Debug.Log("Enemy " + self.GetInstanceID() + " has entered " + name);
    }

    public override void OnExit(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        agent.ResetPath();
        SetIsAgentMovingAnimation(self, agent);
    }

    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        agent.SetDestination(target.transform.position);
        SetIsAgentMovingAnimation(self, agent);
    }
}
