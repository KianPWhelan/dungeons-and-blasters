using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [Tooltip("List of weapons in this holder")]
    [SerializeField]
    private List<Weapon> weapons = new List<Weapon>();

    [Tooltip("Type of object the weapons will hit")]
    [SerializeField]
    private string targetTag;

    public void UseWeapon(int index)
    {
        weapons[index].Use(gameObject, targetTag);
    }

    public void UseWeapon(string weaponName)
    {
        weapons.Find(x => x.name == name);
    }

    public List<Weapon> GetWeapons()
    {
        return weapons;
    }
}
