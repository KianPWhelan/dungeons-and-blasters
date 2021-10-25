using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Damage/Damage")]
public class Damage : ScriptableObject
{
    [Tooltip("Type of damage this damage instance deals")]
    [SerializeField]
    private DamageType damageType;

    [Tooltip("Amount of damage deals")]
    [SerializeField]
    private float damage;

    public void DoDamage(Health hp, float damageMod)
    {
        hp.AdjustHealth(-damage * damageMod, damageType);
    }
}
