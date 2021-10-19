using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/SubAIState")]
public class SubAIState : State
{
    [Tooltip("The EnemyAI that this Sub AI holds")]
    [SerializeField]
    private EnemyAI enemyAi;

    public override void OnEnter(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
    }

    public override void OnExit(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
    }

    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        enemyAi.Tick(self, target, allyTarget, agent, movement);
    }
}
