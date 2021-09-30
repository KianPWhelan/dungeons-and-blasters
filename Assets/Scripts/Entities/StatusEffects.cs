using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    [Tooltip("Status effects currently applied to the entity")]
    [SerializeField]
    private List<Effect> statusEffects = new List<Effect>();

    [Tooltip("Status effect immunities")]
    [SerializeField]
    private List<Effect> immunities = new List<Effect>();

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyStatusEffect(Effect effect)
    {
        if(!statusEffects.Contains(effect))
        {
            statusEffects.Add(effect);
        }
    }

    public bool IsAffectedBy(Effect effect)
    {
        return statusEffects.Contains(effect);
    }
}
