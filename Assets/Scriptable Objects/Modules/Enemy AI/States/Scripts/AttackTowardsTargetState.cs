using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/AttackTowardsTargetState")]
public class AttackTowardsTargetState : State
{
    public override void OnEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        Debug.Log("Enemy " + self.GetInstanceID() + " has entered " + name);
        var weapons = self.GetComponent<WeaponHolder>();
        bool didAttack = false;

        for (int i = 0; i < weapons.GetWeapons().Count; i++)
        {
            bool success = weapons.UseWeapon(i, target.transform.position);

            if (success)
            {
                didAttack = true;
                Debug.Log("");
            }
        }

        if (didAttack)
        {
            SetAttackAnimationTrigger(self);
        }
    }

    public override void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }
}
