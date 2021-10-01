using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class StatusEffects : MonoBehaviour
{
    [Tooltip("Status effects currently applied to the entity")]
    [SerializeField]
    private List<Effect> statusEffects = new List<Effect>();

    [Tooltip("Status effect immunities")]
    [SerializeField]
    private List<Effect> immunities = new List<Effect>();

    private Dictionary<Effect, Container> effectDurations = new Dictionary<Effect, Container>();

    private class Container
    {
        public float durationTime;
        public float intervalTime;

        public Container(float durationTime, float intervalTime)
        {
            this.durationTime = durationTime;
            this.intervalTime = intervalTime;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        ProcessStatusEffects();
    }

    /// <summary>
    /// Applies the provided effect as a status effect to the entity
    /// </summary>
    /// <param name="effect"></param>
    public void ApplyStatusEffect(Effect effect)
    {
        if(!statusEffects.Contains(effect))
        {
            statusEffects.Add(effect);
            if(effectDurations.ContainsKey(effect))
            {
                effectDurations[effect].durationTime = Time.time;
            }

            else
            {
                effectDurations.Add(effect, new Container(Time.time, Time.time));
            }
        }
    }

    /// <summary>
    /// Returns true if the entity is currently affected by the status effect
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool IsAffectedBy(Effect effect)
    {
        return statusEffects.Contains(effect);
    }

    private void ProcessStatusEffects()
    {
        foreach (Effect effect in statusEffects)
        {
            var currentTime = Time.time;

            // If the effect has lasted its duration, remove it
            if(effectDurations[effect].durationTime + effect.GetDuration() >= currentTime)
            {
                statusEffects.Remove(effect);
                continue;
            }
            
            // If effect has reached its proc interval, check immunities and apply
            if(effectDurations[effect].intervalTime + effect.GetInterval() >= currentTime)
            {
                if(immunities.Contains(effect))
                {
                    effect.ApplyEffect(gameObject, gameObject.GetComponent<Health>(), this);
                    effectDurations[effect].intervalTime = currentTime;
                }
            }
        }
    }
}
