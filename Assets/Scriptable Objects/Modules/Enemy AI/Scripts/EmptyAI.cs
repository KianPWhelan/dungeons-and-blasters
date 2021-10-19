using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Plug and play AI that follows and attacks a target if it is within line of sight
/// </summary>
[CreateAssetMenu(menuName = "Modules/AI/Empty AI")]
public class EmptyAI : EnemyAI
{
    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {}
}
