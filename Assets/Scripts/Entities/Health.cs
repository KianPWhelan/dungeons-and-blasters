using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("FloatVariable reference for health point (Leave blank to use normal float)")]
    private FloatVariable health;

    [Tooltip("Normal float, does not exist outside of behavior")]
    [SerializeField]
    private float floatHealth;

    [Tooltip("Entity has infinite health")]
    [SerializeField]
    private bool infiniteHealth = false;

    /// <summary>
    /// Adjusts health value by amount provided, can be negative
    /// </summary>
    /// <param name="amount"></param>
    public void AdjustHealth(float amount)
    {
        if(infiniteHealth)
        {
            return;
        }

        if(health != null)
        {
            health.runtimeValue += amount;
        }

        else
        {
            floatHealth += amount;
        }
    }

    public float GetHealth()
    {
        if(infiniteHealth)
        {
            return Mathf.Infinity;
        }

        if(health != null)
        {
            return health.runtimeValue;
        }

        return floatHealth;
    }
}
