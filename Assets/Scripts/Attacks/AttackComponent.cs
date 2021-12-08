using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : NetworkBehaviour
{
    [Networked]
    [HideInInspector]
    public string validTag { get; set; }
    [Networked]
    [HideInInspector]
    public float damageMod { get; set; }
    [Networked]
    [HideInInspector]
    public Vector3 destination { get; set; }
    [Networked]
    [HideInInspector]
    public bool useDestination { get; set; }

    public virtual void InitNetworkState(string validTag, float damageMod, object destination, NetworkObject owner = null, int weaponIndex = 0, int attackIndex = 0, bool isAlt = false)
    {
    }

    public void CalculateCrit(Attack attack)
    {
        if(attack.canCrit)
        {
            int num = Random.Range(1, 1000);

            if(num <= attack.critChance)
            {
                damageMod *= attack.critMultiplier;
            }
        }
    }
}
