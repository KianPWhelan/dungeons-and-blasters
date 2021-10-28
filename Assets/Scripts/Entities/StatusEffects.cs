using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class StatusEffects : MonoBehaviour
{
    [Tooltip("Status effects currently applied to the entity")]
    [SerializeField]
    private List<Identifier> statusEffects = new List<Identifier>();

    [Tooltip("Status effect immunities")]
    [SerializeField]
    private List<Effect> immunities = new List<Effect>();

    private Dictionary<Identifier, Container> effectDetails = new Dictionary<Identifier, Container>();

    private int idCounter = 0;

    [Serializable]
    private class Identifier
    {
        public Effect effect;
        public float id;

        public Identifier(Effect effect, float id)
        {
            this.effect = effect;
            this.id = id;
        }
    }

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
        var id = idCounter;
        idCounter++;

        if(idCounter > 999999)
        {
            idCounter = 0;
        }

        Identifier identifier = new Identifier(effect, id);

        statusEffects.Add(identifier);
        if(effectDetails.ContainsKey(identifier))
        {
            effectDetails[identifier].durationTime = Time.time;
            effectDetails[identifier].damageMod = damageMod;
        }

        else
        {
            effectDetails.Add(identifier, new Container(Time.time, 0, targetTag, damageMod));
        }
    }

    /// <summary>
    /// Returns true if the entity is currently affected by the status effect
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool CannotApplyMore(Effect effect)
    {
        int count = 0;

        foreach (Identifier e in statusEffects)
        {
            if (e.effect == effect)
            {
                count++;
            }
        }

        // Debug.Log(count + " " + effect.stacks);

        if (count >= effect.stacks)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public bool IsAffectedBy(Effect effect)
    {
        foreach(Identifier status in statusEffects)
        {
            if(status.effect == effect)
            {
                return true;
            }
        }

        return false;
    }

    public void RefreshDuration(Effect effect)
    {
        foreach(Identifier status in statusEffects)
        {
            if(status.effect == effect)
            {
                effectDetails[status].durationTime = Time.time;
            }
        }
    }

    public float GetDamageMod()
    {
        float mod = 1f;

        foreach(Identifier effect in statusEffects)
        {
            if(effect.effect is BuffEffect)
            {
                var temp = (BuffEffect)effect.effect;
                mod *= temp.damageDealtMod;
            }
        }

        return mod;
    }

    public float GetDamageRecievedMod()
    {
        float mod = 1f;

        foreach (Identifier effect in statusEffects)
        {
            if(effect.effect is BuffEffect)
            {
                var temp = (BuffEffect)effect.effect;
                mod *= temp.damageRecievedMod;
            }
        }

        return mod;
    }

    public float GetMoveSpeedMod()
    {
        float mod = 1f;

        foreach (Identifier effect in statusEffects)
        {
            if (effect.effect is BuffEffect)
            {
                var temp = (BuffEffect)effect.effect;
                mod *= temp.moveSpeedMod;
            }
        }

        return mod;
    }

    public float GetResistanceMod(DamageType damageType)
    {
        float mod = 1f;

        foreach(Identifier effect in statusEffects)
        {
            if(effect.effect is BuffEffect)
            {
                var temp = (BuffEffect)effect.effect;

                var res = temp.resistanceMods.Find(x => x.damageType == damageType);

                if(res != null)
                {
                    mod *= res.resistanceMod;
                }
            }
        }

        return mod;
    }

    public bool GetIsStunned()
    {
        foreach(Identifier effect in statusEffects)
        {
            if(effect.effect is BuffEffect)
            {
                var temp = (BuffEffect)effect.effect;

                if(temp.isStun)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void ProcessStatusEffects()
    {
        List<Identifier> remove = null;

        foreach (Identifier effect in statusEffects)
        {
            var currentTime = Time.time;

            // If the effect has lasted its duration, remove it
            if(effectDetails[effect].durationTime + effect.effect.GetDuration() <= currentTime)
            {
                if(remove == null)
                {
                    remove = new List<Identifier>();
                }
                remove.Add(effect);
                continue;
            }
            
            // If effect has reached its proc interval, check immunities and apply
            if(effectDetails[effect].intervalTime + effect.effect.GetInterval() <= currentTime)
            {
                if(!immunities.Contains(effect.effect))
                {
                    effect.effect.ApplyEffect(gameObject, gameObject.GetComponent<Health>(), this, transform.position, transform.rotation, effectDetails[effect].targetTag, effectDetails[effect].damageMod, true);
                    effectDetails[effect].intervalTime = currentTime;
                }
            }
        }

        if(remove != null)
        {
            foreach(Identifier effect in remove)
            {
                statusEffects.Remove(effect);
            }
        }
    }
}
