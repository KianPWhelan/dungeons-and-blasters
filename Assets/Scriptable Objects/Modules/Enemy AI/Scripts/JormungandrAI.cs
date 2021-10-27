using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Modules/AI/JormungandrAI")]
public class JormungandrAI : EnemyAI
{
    [SerializeField]
    private float maxRangeBeforeTP = 30;
    [SerializeField]
    private float teleportTimeout = 1;

    private GameObject targetPlayer;
    private GameObject localSelf;
    private bool onTimeout;
    private float startTimeoutTime;

    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        localSelf = self;

        if(!onTimeout)
        {
            if (target == null)
            {
                target = Helpers.FindClosest(self.transform, "Player");

                if (target == null)
                {
                    return;
                }
            }

            if (Vector3.Distance(self.transform.position, target.transform.position) > maxRangeBeforeTP)
            {
                Debug.Log("Teleporting to player");
                ChooseTargetPlayer();
                TeleportInFrontOfTargetPlayer();
                self.transform.LookAt(target.transform);
                self.transform.rotation = Quaternion.Euler(0, self.transform.rotation.eulerAngles.y, self.transform.rotation.eulerAngles.z);
                onTimeout = true;
                return;
            }

            DoAttacks(target);
        }

        else if(Time.time - startTimeoutTime >= teleportTimeout)
        {
            onTimeout = false;
        }
        
        self.transform.LookAt(target.transform);
        self.transform.rotation = Quaternion.Euler(0, self.transform.rotation.eulerAngles.y, self.transform.rotation.eulerAngles.z);
    }

    private void ChooseTargetPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        int choice = Random.Range(0, players.Length);
        targetPlayer = players[choice];
    }

    private void TeleportInFrontOfTargetPlayer()
    {
        var tile = Helpers.FindClosest(targetPlayer.transform.position + (targetPlayer.transform.forward * 20), "Ground");
        Debug.Log("Teleporting to " + tile.transform.position);
        localSelf.transform.position = tile.transform.position;
    }

    private void DoAttacks(GameObject target)
    {
        var weaponHolder = localSelf.GetComponent<WeaponHolder>();

        // Use shotgun
        weaponHolder.UseWeapon(0, target.transform.position, true);

        // Spawn aura
        weaponHolder.UseWeapon(1);
    }
}
