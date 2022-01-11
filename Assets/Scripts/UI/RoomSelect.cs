using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Com.OfTomorrowInc.DMShooter;
using Newtonsoft.Json;

public class RoomSelect : MonoBehaviour
{
    [SerializeField]
    private List<string> rooms = new List<string>();

    public static string selection;

    private Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        options.Add(new Dropdown.OptionData("No Rooms"));

        dropdown.AddOptions(options);
        //Launcher.clientHash.Add("room", "");
        //PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
        //selection = maps[0];
        ////ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        //Launcher.clientHash.Add("map", selection);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
    }

    public void ChangeSelection()
    {
        selection = rooms[dropdown.value];
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        //Launcher.clientHash["room"] = JsonConvert.DeserializeObject<RoomData>(selection).name;
        //PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
    }

    public void AddMap(string roomJson)
    {
        var roomData = JsonConvert.DeserializeObject<RoomData>(roomJson);

        //Debug.Log(mapData.name);

        List<RoomData> currentMaps = new List<RoomData>();

        foreach (string map in rooms)
        {
            currentMaps.Add(JsonConvert.DeserializeObject<RoomData>(map));
        }

        RoomData found = currentMaps.Find(x => x.name == roomData.name);

        if (found == null)
        {
            rooms.Add(roomJson);
        }

        else
        {
            rooms[currentMaps.IndexOf(found)] = roomJson;
        }

        PopulateOptions();
    }

    private void PopulateOptions()
    {
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach (string map in rooms)
        {
            options.Add(new Dropdown.OptionData(JsonConvert.DeserializeObject<MapData>(map).name));
        }

        dropdown.AddOptions(options);
        selection = rooms[0];
        //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        //Launcher.clientHash["room"] = JsonConvert.DeserializeObject<RoomData>(selection).name;
        //PhotonNetwork.LocalPlayer.SetCustomProperties(Launcher.clientHash);
    }
}
