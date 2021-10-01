using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Attacks/Attack")]
public class Attack : ScriptableObject
{
    [Tooltip("Object pool for attack game objects which are used to gather targets")]
    [SerializeField]
    private ObjectPool attackPool;

    [Tooltip("Effects that the attack inflicts")]
    [SerializeField]
    private List<Effect> effects = new List<Effect>();

    /// <summary>
    /// Performs the actual attack in the scene
    /// </summary>
    public void PerformAttack(GameObject self, string targetTag = "none")
    {
        GameObject attackObject = attackPool.Spawn(self.transform.position, self.transform.rotation);
        var attackScript = attackObject.GetComponent<AttackScript>();
        attackScript.SetAttack(this);
        attackScript.SetValidTag(targetTag);
    }

    /// <summary>
    /// Applies all effects to all targets
    /// </summary>
    /// <param name="targetTag"></param>
    public void ApplyEffects(GameObject target, string targetTag)
    {
        Health health = target.GetComponent<Health>();
        StatusEffects statusEffects = target.GetComponent<StatusEffects>();

        foreach(Effect effect in effects)
        {
            effect.ApplyEffect(target, health, statusEffects, targetTag);
        }
    }
}
