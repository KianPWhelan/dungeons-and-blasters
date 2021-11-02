using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Com.OfTomorrowInc.DMShooter;
using Newtonsoft.Json;

public class MapSelect : MonoBehaviour
{
    [SerializeField]
    private List<string> maps = new List<string>();

    public string selection;

    private Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        options.Add(new Dropdown.OptionData("No Maps"));

        dropdown.AddOptions(options);
        Launcher.clientHash.Add("map", "");
        PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
        //selection = maps[0];
        ////ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        //Launcher.clientHash.Add("map", selection);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
    }

    public void ChangeSelection()
    {
        selection = maps[dropdown.value];
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        Launcher.clientHash["map"] = selection;
        PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
    }

    public void AddMap(string mapJson)
    {
        var mapData = JsonConvert.DeserializeObject<MapData>(mapJson);

        //Debug.Log(mapData.name);

        List<MapData> currentMaps = new List<MapData>();

        foreach (string map in maps)
        {
            currentMaps.Add(JsonConvert.DeserializeObject<MapData>(map));
        }

        MapData found = currentMaps.Find(x => x.name == mapData.name);

        if (found == null)
        {
            maps.Add(mapJson);
        }

        else
        {
            maps[currentMaps.IndexOf(found)] = mapJson;
        }

        PopulateOptions();
    }

    private void PopulateOptions()
    {
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(string map in maps)
        {
            options.Add(new Dropdown.OptionData(JSONTools.LoadMapData(map).name));
        }

        dropdown.AddOptions(options);
        selection = maps[0];
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        Launcher.clientHash["map"] = selection;
        PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
    }
}
