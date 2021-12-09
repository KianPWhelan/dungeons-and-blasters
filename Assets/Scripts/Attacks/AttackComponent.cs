using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : NetworkBehaviour
{
    [HideInInspector]
    public string validTag;

    [HideInInspector]
    public float damageMod;

    [HideInInspector]
    public Vector3 destination;

    [HideInInspector]
    public bool useDestination;

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
