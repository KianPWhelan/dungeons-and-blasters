using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Effects/Effect")]
public class Effect : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("Damage instances that this effect causes")]
    [SerializeField]
    public List<Damage> damages = new List<Damage>();

    [Tooltip("Status effects are applied to the target every defined iteration for the duration")]
    [SerializeField]
    public bool isStatusEffect;

    [Tooltip("If this is a status effect, how often it is applied to the target in seconds (0 for every frame)")]
    [SerializeField]
    public FloatVariable interval;

    [Tooltip("If this is a status effect, how long it lasts in seconds")]
    [SerializeField]
    public FloatVariable duration;

    [Tooltip("Additional effects that this effect applies")]
    [SerializeField]
    public List<Effect> recursiveEffects = new List<Effect>();

    private bool warningDisplayed;

    /// <summary>
    /// Applies all of the logic relating to the effect on the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="health"></param>
    /// <param name="statusEffects"></param>
    /// <param name="targetTag"></param>
    public virtual void ApplyEffect(GameObject target, Health health, StatusEffects statusEffects, Vector3? location, Quaternion? rotation, string targetTag = "none", float damageMod = 1)
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
                damage.DoDamage(health, damageMod);
            }
        }

        else
        {
            Debug.LogWarning("Target of effect has no health behavior");
        }

        // Apply recursive effects
        foreach(Effect effect in recursiveEffects)
        {
            effect.ApplyEffect(target, health, statusEffects, location, rotation, targetTag);
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

    public void OnBeforeSerialize()
    {
        if (recursiveEffects.Contains(this) && !warningDisplayed)
        {
            Debug.LogError("Effect " + name + " calls itself recursively, this will cause extreme lag, stack overflows, and likely crash the editor");
            warningDisplayed = true;
        }
    }

    public void OnAfterDeserialize()
    {
        warningDisplayed = false;
    }
}
