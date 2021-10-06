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
        var weapons = self.GetComponent<WeaponHolder>();
        for(int i = 0; i < weapons.GetWeapons().Count; i++)
        {
            weapons.UseWeapon(i);
        }
    }

    public override void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }
}
