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
    private List<string> targetTags = new List<string>();

    public bool UseWeapon(int index, Vector3? destination = null, bool useDestination = false)
    {
        if(!useDestination)
        {
            destination = Vector3.negativeInfinity;
        }

        return weapons[index].Use(gameObject, targetTags[index], destination);
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
