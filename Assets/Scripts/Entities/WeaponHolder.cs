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

    private bool inSpinup;

    public bool isPlayer;

    public bool UseWeapon(int index, Vector3? destination = null, bool useDestination = false)
    {
        if(!useDestination)
        {
            destination = Vector3.negativeInfinity;
        }

        if(isPlayer && weapons[index].hasSpinup && !inSpinup)
        {
            inSpinup = true;
            StartCoroutine(Spinup(index, weapons[index].spinupTime, destination, useDestination));
            return true;
        }

        else if(inSpinup)
        {
            return false;
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

    public void AddWeapon(Weapon weapon, string targetTag)
    {
        weapons.Add(weapon);
        targetTags.Add(targetTag);
    }

    private IEnumerator Spinup(int index, float delay, Vector3? destination = null, bool useDestination = false)
    {
        yield return new WaitForSeconds(delay);

        while(Input.GetKey(KeyCode.Mouse0))
        {
            weapons[index].Use(gameObject, targetTags[index], destination);
            yield return new WaitForEndOfFrame();
        }

        inSpinup = false;
    }
}
