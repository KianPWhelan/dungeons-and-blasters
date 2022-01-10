using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.UI;

public class RoomList : MonoBehaviour
{
    [SerializeField]
    private Toggle hostToggle;

    [SerializeField]
    private Dropdown roomSelector;

    [SerializeField]
    private NetworkRunner runner;

    private List<SessionInfo> sessions = new List<SessionInfo>();

    public void Start()
    {
        runner.JoinSessionLobby(SessionLobby.Custom, "Custom Lobby");
    }

    public void UpdateRoomList(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Sessions Recieved: " + sessionList.Count);
        sessions = sessionList;
    }

    public void RefreshDropdown()
    {
        roomSelector.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(SessionInfo session in sessions)
        {
            options.Add(new Dropdown.OptionData(session.Name));
        }

        roomSelector.AddOptions(options);
    }

    public void SelectRoom()
    {
        if(!hostToggle.isOn)
        {
            JoinSettings.roomName = roomSelector.options[roomSelector.value].text;
        }
    }
}
