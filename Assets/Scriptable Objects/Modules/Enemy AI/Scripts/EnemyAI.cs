using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : ScriptableObject
{
    public abstract void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement);
}
