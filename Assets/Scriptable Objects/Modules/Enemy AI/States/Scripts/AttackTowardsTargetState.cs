using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/AttackTowardsTargetState")]
public class AttackTowardsTargetState : State
{
    public Vector3 offset;

    public bool rotateTowards;

    public override void OnEnter(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        Debug.Log("Enemy " + self.GetInstanceID() + " has entered " + name);
        var weapons = self.GetComponent<WeaponHolder>();
        bool didAttack = false;

        if(target == null)
        {
            return;
        }

        if(rotateTowards)
        {
            self.transform.rotation = Quaternion.LookRotation((target.transform.position - self.transform.position).normalized);
        }

        for (int i = 0; i < weapons.GetWeapons().Count; i++)
        {
            bool success = weapons.UseWeapon(i, target.transform.position + offset, true);

            if (success)
            {
                didAttack = true;
            }
        }

        if (didAttack)
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
