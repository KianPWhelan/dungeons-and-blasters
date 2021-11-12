using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[CreateAssetMenu(menuName = "Modules/AI/StandardAI")]
public class StandardAI : EnemyAI
{
    [SerializeField]
    private Vector3 offset;

    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement, EnemyGeneric enemyGeneric = null)
    {
        TryAndAttackWithAllWeapons(self, target, enemyGeneric);

        if(enemyGeneric.canAggro && target != null)
        {
            Aggro(self, target, enemyGeneric);
        }

        else if(enemyGeneric.isAggro && enemyGeneric.isFollowing && target == null)
        {
            Debug.Log("Canceling follow");
            enemyGeneric.isAggro = false;
            enemyGeneric.canAggro = true;
            enemyGeneric.CancelFollow();
        }
    }

    private void TryAndAttackWithAllWeapons(GameObject self, GameObject target, EnemyGeneric enemyGeneric)
    {
        if(target == null)
        {
            return;
        }

        self.transform.rotation = Quaternion.LookRotation((target.transform.position - self.transform.position).normalized);
        bool success = false;

        for (int i = 0; i < enemyGeneric.weaponHolder.GetWeapons().Count; i++)
        {
            if(Vector3.Distance(self.transform.position, target.transform.position) <= enemyGeneric.ranges[i])
            {
                success = enemyGeneric.weaponHolder.UseWeapon(i, target.transform.position + offset, true);
            }
        }

        if(success)
        {
            enemyGeneric.photonView.RPC("SetAnimationTrigger", RpcTarget.All, "attack");
        }
    }

    private void Aggro(GameObject self, GameObject target, EnemyGeneric enemyGeneric)
    {
        float dist = Vector3.Distance(self.transform.position, target.transform.position);
        enemyGeneric.isAggro = true;

        if (dist > enemyGeneric.desiredRange)
        {
            enemyGeneric.FollowEntity(target);
        }

        else
        {
            enemyGeneric.ClearPath();
            enemyGeneric.CancelFollow();
        }
    }
}
