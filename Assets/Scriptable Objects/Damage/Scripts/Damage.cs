using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : ScriptableObject
{
    [Tooltip("Type of damage this damage instance deals")]
    [SerializeField]
    private DamageType damageType;

    [Tooltip("Amount of damage deals")]
    [SerializeField]
    private FloatVariable damage;

    public void DoDamage(Health hp)
    {
        hp.AdjustHealth(-damage.runtimeValue);
    }
}
