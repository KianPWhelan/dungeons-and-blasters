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

    /// <summary>
    /// Performs the actual attack in the scene
    /// </summary>
    public void PerformAttack(GameObject self, string targetTag = "none", bool useSelfAsParent = true, Vector3? destination = null)
    {
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
            info = new object[] { self.GetComponent<PhotonView>().ViewID, targetTag, damageMod, destination };
        }

        else
        {
            info = new object[] { null, targetTag, damageMod, destination };
        }
        
        GameObject attackObject = PhotonNetwork.Instantiate(attack.name, self.transform.position, self.transform.rotation, 0, info);
        attackObject.transform.SetParent(self.transform);
        var attackScript = attackObject.GetComponent<AttackScript>();
        attackScript.SetAttack(this);
        attackScript.SetValidTag(targetTag);
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
        GameObject attackObject = PhotonNetwork.Instantiate(attack.name, selfPosition, selfRotation, 0, info);
        var attackScript = attackObject.GetComponent<AttackScript>();
        attackScript.SetAttack(this);
        attackScript.SetValidTag(targetTag);
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
