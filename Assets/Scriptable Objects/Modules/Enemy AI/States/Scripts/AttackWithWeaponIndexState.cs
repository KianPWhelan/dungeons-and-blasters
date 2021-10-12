using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/AttackWithWeaponIndexState")]
public class AttackWithWeaponIndexState : State
{
    [Tooltip("The index in the enemys WeaponHolder that we want to use")]
    public int weaponIndex;

    public override void OnEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        Debug.Log("Enemy " + self.GetInstanceID() + " has entered " + name);
        var weapons = self.GetComponent<WeaponHolder>();
        bool didAttack = false;

        didAttack = weapons.UseWeapon(weaponIndex);

        if (didAttack)
        {
            SetAttackAnimationTrigger(self, weaponIndex);
        }
    }

    public override void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
    }
}
