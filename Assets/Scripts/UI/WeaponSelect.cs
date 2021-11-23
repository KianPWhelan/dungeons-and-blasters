using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Com.OfTomorrowInc.DMShooter;

public class WeaponSelect : MonoBehaviour
{
    [SerializeField]
    private List<WeaponDeprecated> weapons = new List<WeaponDeprecated>();

    [SerializeField]
    private Inventory inventory;

    public WeaponDeprecated selection;

    private Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(WeaponDeprecated weapon in weapons)
        {
            options.Add(new Dropdown.OptionData(weapon.name));
        }

        dropdown.AddOptions(options);
        selection = weapons[0];
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        Launcher.clientHash.Add("weapon", selection.name);
        PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
    }

    public void ChangeSelection()
    {
        selection = weapons[dropdown.value];
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        Launcher.clientHash["weapon"] = selection.name;
        PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
    }

    public void OnDisable()
    {
        inventory.weapons.Add(selection);
    }
}
