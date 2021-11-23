using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    public List<WeaponDeprecated> weapons = new List<WeaponDeprecated>();
}
