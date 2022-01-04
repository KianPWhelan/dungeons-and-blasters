using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[CreateAssetMenu(menuName = "Modules/Weapons/Damage/Damage")]
public class Damage : ScriptableObject
{
    [Tooltip("Type of damage this damage instance deals")]
    [SerializeField]
    private DamageType damageType;

    [Tooltip("Amount of damage deals")]
    [SerializeField]
    private float damage;

    public void DoDamage(Health hp, float damageMod, float overheal = 0f, NetworkObject owner = null, float lifesteal = 0f)
    {
        float dMod = 1;

        if(!damageType.immuneToDamageMod)
        {
            dMod = damageMod;
        }

        hp.AdjustHealth(-damage * dMod, damageType, overheal, owner, lifesteal);
    }
}
