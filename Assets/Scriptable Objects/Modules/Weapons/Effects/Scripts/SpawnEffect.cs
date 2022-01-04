using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[CreateAssetMenu(menuName = "Modules/Weapons/Effects/SpawnEffect")]
public class SpawnEffect : Effect
{
    [Tooltip("Prefab of unit to spawn")]
    [SerializeField]
    private NetworkObject unit;

    [Tooltip("Spawn when the target is dead (If not set as status effect, will only apply on the frame the target was hit)")]
    [SerializeField]
    private bool spawnOnTargetDeath;

    private NetworkRunner runner;

    private EnemyManager enemyManager;

    private bool addToEnemyList;

    public override void ApplyEffect(GameObject target, Health health, StatusEffects statusEffects, Vector3? location, Quaternion? rotation, string targetTag = "none", float damageMod = 1, bool isProc = false, float id = -1)
    {
        //Debug.Log("In spawn effect");
        //Debug.Log("Is Proc: " + isProc);

        if(runner == null)
        {
            runner = FindObjectOfType<NetworkRunner>();
        }

        if(unit.TryGetComponent(out EnemyGeneric e))
        {
            enemyManager = FindObjectOfType<EnemyManager>();
        }

        var tag = targetTag;

        if (useOverwriteTag)
        {
            tag = overwriteTag;
        }

        if (statusEffects != null && isStatusEffect && !statusEffects.CannotApplyMore(this) && !isProc && (target.tag == tag || tag == "none"))
        {
            // Apply status effect if not already applied
            statusEffects.ApplyStatusEffect(this, damageMod, tag);

            // We can now leave it to the status effect behavior to apply the rest of the effects
            return;
        }

        else if (statusEffects == null)
        {
            Debug.LogWarning("Target of effect has no status effects behavior");
        }

        if (location == null)
        {
            Debug.LogWarning("Called spawn effect with no location provided");
        }

        //Debug.Log("Is Dead " + health.isDead);

        if(!spawnOnTargetDeath || (health != null && health.isDead))
        {
            if(spawnOnTargetDeath && health != null)
            {
                location = health.transform.position;
            }

            //Debug.Log("Location " + location);
            var newUnit = runner.Spawn(unit, location.GetValueOrDefault(), rotation.GetValueOrDefault());
            enemyManager.AddEnemy(newUnit);
        }
    }
}
