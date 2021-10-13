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

    private Spawner spawner;

    /// <summary>
    /// Performs the actual attack in the scene
    /// </summary>
    public void PerformAttack(GameObject self, string targetTag = "none", bool useSelfAsParent = true, Vector3? destination = null)
    {
        Debug.Log("Destination: " + destination.GetValueOrDefault());

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
            info = new object[] { self.GetComponent<PhotonView>().ViewID, targetTag, damageMod, destination.GetValueOrDefault() };
        }

        else
        {
            info = new object[] { null, targetTag, damageMod, destination.GetValueOrDefault() };
        }

        if(destination.GetValueOrDefault().x == Vector3.negativeInfinity.x)
        {
            info[3] = null;
        }
        
        spawner.Spawn(attack.name, self.transform.position, self.transform.rotation, info);
    }

    /// <summary>
    /// Performs the actual attack in the scene
    /// </summary>
    public void PerformAttack(Vector3 selfPosition, Quaternion selfRotation, float damageMod, string targetTag = "none")
    {
        Debug.Log("Performing attack");
        Debug.Log(attack);
        object[] info;
        info = new object[] { null, targetTag };
        spawner.Spawn(attack.name, selfPosition, selfRotation, info);
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
