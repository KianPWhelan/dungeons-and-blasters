using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [Tooltip("List of weapons in this holder")]
    [SerializeField]
    private List<GameObject> weaponPrefabs = new List<GameObject>();

    [Tooltip("Type of object the weapons will hit")]
    [SerializeField]
    private List<string> targetTags = new List<string>();

    private List<Weapon> weaponScripts = new List<Weapon>();

    private bool inSpinup;

    private float spinupIndex;

    public bool isPlayer;

    [HideInInspector]
    public FPSCamera cam;

    public void Start()
    {
        for(int i = 0; i < weaponPrefabs.Count; i++)
        {
            var weapon = weaponPrefabs[i];
            var newWeapon = Instantiate(weapon, transform);
            weaponScripts.Add(newWeapon.GetComponent<Weapon>());
        }

        if(isPlayer)
        {
            cam = FindObjectOfType<FPSCamera>();
        }
    }

    public bool UseWeapon(int index, Vector3? destination = null, bool useDestination = false, bool altAttack = false)
    {
        //if(!useDestination)
        //{
        //    destination = Vector3.negativeInfinity;
        //}

        //if(isPlayer && weapons[index].isOverheated)
        //{
        //    return false;
        //}

        //if(inSpinup && spinupIndex != index)
        //{
        //    inSpinup = false;
        //}

        //if(isPlayer && weapons[index].hasSpinup && !inSpinup)
        //{
        //    inSpinup = true;
        //    spinupIndex = index;
        //    StartCoroutine(Spinup(index, weapons[index].spinupTime, destination, useDestination));
        //    return true;
        //}

        //else if(inSpinup)
        //{
        //    return false;
        //}

        if(altAttack)
        {
            
        }

        return weaponScripts[index].Use(gameObject, targetTags[index], useDestination, destination, useRotation: true, rotation: cam.transform.rotation, weaponHolder: this);
    }

    //public void UseWeapon(string weaponName)
    //{
    //    weaponScripts.Find(x => x.name == name);
    //}

    public List<Weapon> GetWeapons()
    {
        return weaponScripts;
    }

    public void AddWeapon(Weapon weapon, string targetTag)
    {
        weaponScripts.Add(weapon);
        targetTags.Add(targetTag);
    }

    //private IEnumerator Spinup(int index, float delay, Vector3? destination = null, bool useDestination = false)
    //{
    //    float startTime = Time.time;
    //    yield return new WaitUntil(() => Time.time - startTime >= delay || Input.GetKeyUp(KeyCode.Mouse0));
    //    startTime = Time.time;

    //    while(Input.GetKey(KeyCode.Mouse0))
    //    {
    //        if(weapons[index].canOverheat && Time.time - startTime >= weapons[index].overheatTime)
    //        {
    //            weapons[index].isOverheated = true;
    //            StartCoroutine(RunOverheatCooldown(weapons[index]));
    //            break;
    //        }

    //        weapons[index].Use(gameObject, targetTags[index], destination);
    //        yield return new WaitForEndOfFrame();
    //    }

    //    inSpinup = false;
    //}

    //private IEnumerator RunOverheatCooldown(WeaponDeprecated weapon)
    //{
    //    yield return new WaitForSeconds(weapon.overheatRecoveryTime);
    //    weapon.isOverheated = false;
    //}
}
