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

    private Dictionary<Effect, Container> effectDetails = new Dictionary<Effect, Container>();

    private class Container
    {
        public float durationTime;
        public float intervalTime;
        public string targetTag;
        public float damageMod;

        public Container(float durationTime, float intervalTime, string targetTag, float damageMod)
        {
            this.durationTime = durationTime;
            this.intervalTime = intervalTime;
            this.targetTag = targetTag;
            this.damageMod = damageMod;
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
    public void ApplyStatusEffect(Effect effect, float damageMod, string targetTag = "none")
    {
        Debug.Log("Applying Status Effect");
        statusEffects.Add(effect);
        if(effectDetails.ContainsKey(effect))
        {
            effectDetails[effect].durationTime = Time.time;
            effectDetails[effect].damageMod = damageMod;
        }

        else
        {
            effectDetails.Add(effect, new Container(Time.time, 0, targetTag, damageMod));
        }
    }

    /// <summary>
    /// Returns true if the entity is currently affected by the status effect
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool IsAffectedBy(Effect effect)
    {
        int count = 0;

        foreach (Effect e in statusEffects)
        {
            if (e == effect)
            {
                count++;
            }
        }

        Debug.Log(count + " " + effect.stacks);

        if (count >= effect.stacks)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public float GetDamageMod()
    {
        float mod = 1f;

        foreach(Effect effect in statusEffects)
        {
            if(effect is BuffEffect)
            {
                var temp = (BuffEffect)effect;
                mod *= temp.damageDealtMod;
            }
        }

        return mod;
    }

    public float GetDamageRecievedMod()
    {
        float mod = 1f;

        foreach (Effect effect in statusEffects)
        {
            if(effect is BuffEffect)
            {
                var temp = (BuffEffect)effect;
                mod *= temp.damageRecievedMod;
            }
        }

        return mod;
    }

    public float GetMoveSpeedMod()
    {
        float mod = 1f;

        foreach (Effect effect in statusEffects)
        {
            if (effect is BuffEffect)
            {
                var temp = (BuffEffect)effect;
                mod *= temp.moveSpeedMod;
            }
        }

        return mod;
    }

    private void ProcessStatusEffects()
    {
        List<Effect> remove = null;

        foreach (Effect effect in statusEffects)
        {
            var currentTime = Time.time;

            // If the effect has lasted its duration, remove it
            if(effectDetails[effect].durationTime + effect.GetDuration() <= currentTime)
            {
                if(remove == null)
                {
                    remove = new List<Effect>();
                }
                remove.Add(effect);
                continue;
            }
            
            // If effect has reached its proc interval, check immunities and apply
            if(effectDetails[effect].intervalTime + effect.GetInterval() <= currentTime)
            {
                if(!immunities.Contains(effect))
                {
                    effect.ApplyEffect(gameObject, gameObject.GetComponent<Health>(), this, transform.position, transform.rotation, effectDetails[effect].targetTag, effectDetails[effect].damageMod, true);
                    effectDetails[effect].intervalTime = currentTime;
                }
            }
        }

        if(remove != null)
        {
            foreach(Effect effect in remove)
            {
                statusEffects.Remove(effect);
            }
        }
    }
}
