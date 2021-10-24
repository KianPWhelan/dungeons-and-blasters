using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(menuName = "Modules/Weapons/Attacks/Attack")]
public class Attack : ScriptableObject
{
    [Tooltip("Object pool for attack game objects which are used to gather targets")]
    [SerializeField]
    private GameObject attack;

    [Tooltip("Effects that the attack inflicts")]
    [SerializeField]
    private List<Effect> effects = new List<Effect>();

    [Tooltip("Set as true to use the tag provided below instead of the tag provided by the weapon user")]
    [SerializeField]
    private bool useOverwriteTag;

    [Tooltip("If Use Override Tag is set to true, the attack will apply use this as the valid tag instead of the tag provided by the weapon user")]
    [SerializeField]
    private string overwriteTag;

    private Spawner spawner;

    /// <summary>
    /// Performs the actual attack in the scene
    /// </summary>
    public void PerformAttack(GameObject self, float delay, string targetTag = "none", bool useSelfAsParent = true, Vector3? destination = null)
    {
        Debug.Log("Using Attack " + name);
        var tag = targetTag;

        if(useOverwriteTag)
        {
            tag = overwriteTag;
        }

        if(spawner == null)
        {
            var spawnerObj = GameObject.FindGameObjectWithTag("Spawner");
            spawner = spawnerObj.GetComponent<Spawner>();
        }

        Debug.Log("Performing attack");
        Debug.Log(attack);
        object[] info;

        float damageMod = 1;

        if(self.TryGetComponent(out StatusEffects s))
        {
            damageMod = s.GetDamageMod();
        }

        if(useSelfAsParent)
        {
            info = new object[] { self.GetComponent<PhotonView>().ViewID, tag, damageMod, destination.GetValueOrDefault() };
        }

        else
        {
            info = new object[] { null, tag, damageMod, destination.GetValueOrDefault() };
        }

        if(destination.GetValueOrDefault().x == Vector3.negativeInfinity.x)
        {
            info[3] = null;
        }
        
        spawner.Spawn(attack.name, self.transform.position, self.transform.rotation, info, delay);
    }

    /// <summary>
    /// Performs the actual attack in the scene
    /// </summary>
    public void PerformAttack(Vector3 selfPosition, Quaternion selfRotation, float damageMod, string targetTag = "none", float delay = 0, Vector3? destination = null)
    {
        var tag = targetTag;

        if (useOverwriteTag)
        {
            tag = overwriteTag;
        }

        if (spawner == null)
        {
            var spawnerObj = GameObject.FindGameObjectWithTag("Spawner");
            spawner = spawnerObj.GetComponent<Spawner>();
        }

        Debug.Log("Performing attack");
        Debug.Log(attack);
        object[] info;
        info = new object[] { null, tag, damageMod, destination.GetValueOrDefault() };
        spawner.Spawn(attack.name, selfPosition, selfRotation, info, delay);
    }

    /// <summary>
    /// Applies all effects to all targets
    /// </summary>
    /// <param name="targetTag"></param>
    public void ApplyEffects(GameObject target, string targetTag, Vector3? location = null, Quaternion? rotation = null, float damageMod = 1)
    {
        Health health = null;
        StatusEffects statusEffects = null;
        if(target != null)
        {
            health = target.GetComponent<Health>();
            statusEffects = target.GetComponent<StatusEffects>();
        }
        
        Debug.Log("Damage Mod: " + damageMod);

        foreach(Effect effect in effects)
        {
            effect.ApplyEffect(target, health, statusEffects, location, rotation, targetTag, damageMod);
        }
    }
}
