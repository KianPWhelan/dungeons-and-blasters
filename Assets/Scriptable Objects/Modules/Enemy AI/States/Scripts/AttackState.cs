using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/AttackState")]
public class AttackState : State
{
    public override void OnEnter(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        Debug.Log("Enemy " + self.GetInstanceID() + " has entered " + name);
        var weapons = self.GetComponent<WeaponHolder>();
        bool didAttack = false;

        for(int i = 0; i < weapons.GetWeapons().Count; i++)
        {
            bool success = weapons.UseWeapon(i);

            if(success)
            {
                didAttack = true;
            }
        }

        if(didAttack)
        {
            SetAttackAnimationTrigger(self);
        }      
    }

    public override void OnExit(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
    }

    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
    }
}
