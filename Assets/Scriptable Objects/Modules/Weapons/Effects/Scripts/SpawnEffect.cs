using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(menuName = "Modules/Weapons/Effects/SpawnEffect")]
public class SpawnEffect : Effect
{
    [Tooltip("Prefab of unit to spawn")]
    [SerializeField]
    private GameObject unit;

    [Tooltip("Spawn when the target is dead (If not set as status effect, will only apply on the frame the target was hit)")]
    [SerializeField]
    private bool spawnOnTargetDeath;

    public override void ApplyEffect(GameObject target, Health health, StatusEffects statusEffects, Vector3? location, Quaternion? rotation, string targetTag = "none", float damageMod = 1, bool isProc = false)
    {
        Debug.Log("In spawn effect");
        Debug.Log("Is Proc: " + isProc);

        var tag = targetTag;

        if (useOverwriteTag)
        {
            tag = overwriteTag;
        }

        if (statusEffects != null && isStatusEffect && !statusEffects.IsAffectedBy(this) && !isProc && (target.tag == tag || tag == "none"))
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
            Debug.LogError("Called spawn effect with no location provided");
        }

        if(!spawnOnTargetDeath || (health != null && health.isDead))
        {
            PhotonNetwork.Instantiate(unit.name, location.GetValueOrDefault(), rotation.GetValueOrDefault(), 0);
        }
    }
}
