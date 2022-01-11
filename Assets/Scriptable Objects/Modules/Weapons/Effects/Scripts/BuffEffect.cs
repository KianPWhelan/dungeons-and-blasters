using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Effects/Buff Effect")]
public class BuffEffect : Effect
{
    public float damageDealtMod = 1;

    public float damageRecievedMod = 1;

    public float moveSpeedMod = 1;

    [Tooltip("Different from effect specific lifesteal. Causes all damage dealt while the buff is active to lifesteal for the amount (e.g. 0.1 lifesteal = 10%). Lifesteal stacks additively")]
    public float buffLifesteal;

    public bool isStun;

    public List<Health.ResistanceContainer> resistanceMods = new List<Health.ResistanceContainer>();
}
