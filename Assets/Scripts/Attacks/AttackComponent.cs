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

    public virtual void InitNetworkState(string validTag, float damageMod, object destination)
    {
    }
}
