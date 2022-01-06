using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Effects/Effect")]
public class Effect : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("Damage instances that this effect causes")]
    [SerializeField]
    public List<Damage> damages = new List<Damage>();

    [Tooltip("If the damage is negative (aka healing), how far over the starting health it can heal (also affects lifesteal, different from Buff Lifesteal)")]
    public float overheal;

    [Tooltip("Percentage of damage dealt by this effect to be returned to the caster as health, is affected by overheal (e.g. 0.1 lifesteal is 10%)")]
    public float lifesteal;

    [Tooltip("Status effects are applied to the target every defined iteration for the duration")]
    [SerializeField]
    public bool isStatusEffect;

    [Tooltip("If this is a status effect, how often it is applied to the target in seconds (0 for every frame)")]
    [SerializeField]
    public FloatVariable interval;

    [Tooltip("If this is a status effect, how long it lasts in seconds")]
    [SerializeField]
    public FloatVariable duration;

    [Tooltip("If this is a status effect, how many times it stacks")]
    public int stacks;

    [Tooltip("When status effect is applied, resets duration if already applied (will reset duration of stack)")]
    public bool refreshDuration;

    [Tooltip("If this is a status effect, will make this effect not apply its damage/recursive effects until <Num Stacks For Application> stacks have been applied")]
    public bool stackAmountIsRequiredForApplication;

    [Tooltip("List of status effects and how many of them are required if Stack Amount Is Required For Application is set to true")]
    public List<StackApplicationData> effectStackApplicationData = new List<StackApplicationData>();

    [Tooltip("If status effect, will remove itself from the target after damage/recursive effects are applied once")]
    public bool onlyApplyOnce;

    [Tooltip("Additional effects that this effect applies")]
    [SerializeField]
    public List<Effect> recursiveEffects = new List<Effect>();

    [Tooltip("Set as true to use the tag provided below instead of the tag provided by the weapon user")]
    [SerializeField]
    public bool useOverwriteTag;

    [Tooltip("If Use Override Tag is set to true, the attack will apply use this as the valid tag instead of the tag provided by the weapon user")]
    [SerializeField]
    public string overwriteTag;

    private bool warningDisplayed;

    [System.Serializable]
    public class StackApplicationData
    {
        [Tooltip("The type of status effects required")]
        public Effect effect;

        [Tooltip("How many stacks are needed")]
        public int numStacksForApplication;

        [Tooltip("When the main effect relying on this effect applies its damage/recursive effect, remove all stacks of this effect from the target")]
        public bool removeEffectWhenTriggered;
    }

    /// <summary>
    /// Applies all of the logic relating to the effect on the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="health"></param>
    /// <param name="statusEffects"></param>
    /// <param name="targetTag"></param>
    public virtual void ApplyEffect(GameObject target, Health health, StatusEffects statusEffects, Vector3? location, Quaternion? rotation, string targetTag = "none", float damageMod = 1, bool isProc = false, float id = -1, NetworkObject owner = null)
    {
        var tag = targetTag;

        if (useOverwriteTag)
        {
            tag = overwriteTag;
        }

        if (statusEffects != null && isStatusEffect && refreshDuration && statusEffects.IsAffectedBy(this) && !isProc && (target.tag == tag || tag == "none"))
        {
            Debug.Log("Refreshing");
            statusEffects.RefreshDuration(this);
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

        else if (isStatusEffect && !isProc)
        {
            return;
        }

        if(stackAmountIsRequiredForApplication && !HasEnoughStacks(statusEffects))
        {
            return;
        }

        if (health != null && (target.tag == tag || tag == "none"))
        {
            // Apply damage first
            foreach (Damage damage in damages)
            {
                damage.DoDamage(health, damageMod, overheal, owner, lifesteal);
            }
        }

        else
        {
            Debug.Log("Target of effect has no health behavior or incorrect tag");
        }

        // Apply recursive effects
        foreach (Effect effect in recursiveEffects)
        {
            effect.ApplyEffect(target, health, statusEffects, location, rotation, tag);
        }

        if(isStatusEffect && onlyApplyOnce)
        {
            statusEffects.RemoveEffect(this, id);

            if(stackAmountIsRequiredForApplication)
            {
                foreach(StackApplicationData data in effectStackApplicationData)
                {
                    statusEffects.RemoveEffect(data.effect, removeAllStacks: true);
                }
            }
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

    public bool HasEnoughStacks(StatusEffects statusEffects)
    {
        foreach(StackApplicationData data in effectStackApplicationData)
        {
            if(statusEffects.GetStacks(data.effect) < data.numStacksForApplication)
            {
                return false;
            }
        }

        return true;
    }
}
