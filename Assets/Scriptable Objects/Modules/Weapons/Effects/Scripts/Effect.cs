using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Effects/Effect")]
public class Effect : ScriptableObject
{
    [Tooltip("Damage instances that this effect causes")]
    [SerializeField]
    private List<Damage> damages = new List<Damage>();

    [Tooltip("Status effects are applied to the target every defined iteration for the duration")]
    [SerializeField]
    private bool isStatusEffect;

    [Tooltip("If this is a status effect, how often it is applied to the target in seconds (0 for every frame)")]
    [SerializeField]
    private FloatVariable interval;

    [Tooltip("If this is a status effect, how long it lasts in seconds")]
    [SerializeField]
    private FloatVariable duration;

    [Tooltip("Additional effects that this effect applies")]
    [SerializeField]
    private List<Effect> recursiveEffects = new List<Effect>();

    /// <summary>
    /// Applies all of the logic relating to the effect on the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="health"></param>
    /// <param name="statusEffects"></param>
    /// <param name="targetTag"></param>
    public virtual void ApplyEffect(GameObject target, Health health, StatusEffects statusEffects, string targetTag = "none")
    {
        if(statusEffects != null && isStatusEffect && !statusEffects.IsAffectedBy(this))
        {
            // Apply status effect if not already applied
            statusEffects.ApplyStatusEffect(this, targetTag);

            // We can now leave it to the status effect behavior to apply the rest of the effects
            return;
        }

        else if(statusEffects == null)
        {
            Debug.LogWarning("Target of effect has no status effects behavior");
        }

        if(health != null)
        {
            // Apply damage first
            foreach (Damage damage in damages)
            {
                damage.DoDamage(health);
            }
        }

        else
        {
            Debug.LogWarning("Target of effect has no health behavior");
        }

        // Apply recursive effects
        foreach(Effect effect in recursiveEffects)
        {
            ApplyEffect(target, health, statusEffects);
        }
    }

    /// <summary>
    /// Returns the delay between procs for the status effect in seconds
    /// </summary>
    /// <returns></returns>
    public float GetInterval()
    {
        return interval.runtimeValue;
    }

    /// <summary>
    /// Returns the duration of the status effect in seconds
    /// </summary>
    /// <returns></returns>
    public float GetDuration()
    {
        return duration.runtimeValue;
    }
}
