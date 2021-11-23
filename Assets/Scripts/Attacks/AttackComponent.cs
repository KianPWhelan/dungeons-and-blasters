using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackComponent : NetworkBehaviour
{
    public string validTag;
    public float damageMod;

    public virtual void InitNetworkState(string validTag, float damageMod)
    {

    }
}
