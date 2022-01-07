using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.OfTomorrowInc.DMShooter;

public class WeaponSelect : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> weapons = new List<GameObject>();

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private bool isSecondarySelection;

    public static GameObject selection;

    public static GameObject selection2;

    private Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(GameObject weapon in weapons)
        {
            options.Add(new Dropdown.OptionData(weapon.name));
        }

        dropdown.AddOptions(options);
        if (isSecondarySelection)
        {
            selection2 = weapons[0];
        }

        else
        {
            selection = weapons[0];
        }
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
    }

    public void ChangeSelection()
    {
        if(isSecondarySelection)
        {
            selection2 = weapons[dropdown.value];
        }

        else
        {
            selection = weapons[dropdown.value];
        }
        
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
    }

    public void OnDisable()
    {
        //inventory.weapons.Add(selection);
    }
}
