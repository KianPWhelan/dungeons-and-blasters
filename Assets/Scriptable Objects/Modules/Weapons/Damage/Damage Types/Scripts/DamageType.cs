using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Damage/Damage Type")]
public class DamageType : ScriptableObject
{
    public bool immuneToStatusMod;
    public bool immuneToResistanceMod;
    public bool immuneToDamageMod;
}
