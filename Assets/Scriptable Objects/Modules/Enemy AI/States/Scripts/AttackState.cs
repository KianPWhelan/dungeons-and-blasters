using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/AttackState")]
public class AttackState : State
{
    public override string name
    {
        get
        {
            return "AttackState";
        }
    }

    public override void OnEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        Debug.Log("Enemy " + self.GetInstanceID() + " has entered " + name);
        self.GetComponent<WeaponHolder>().UseWeapon(0);
    }

    public override void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }
}
