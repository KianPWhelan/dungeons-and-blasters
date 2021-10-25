using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Effects/Buff Effect")]
public class BuffEffect : Effect
{
    public float damageDealtMod = 1;

    public float damageRecievedMod = 1;

    public float moveSpeedMod = 1;

    public bool isStun;

    public List<Health.ResistanceContainer> resistanceMods = new List<Health.ResistanceContainer>();
}
