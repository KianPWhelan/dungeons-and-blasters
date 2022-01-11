using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostRoom : MonoBehaviour
{
    [SerializeField]
    private InputField roomName;

    [SerializeField]
    private Toggle hostToggle;

    [SerializeField]
    private GameObject sessionSelect;

    public void HostSettings()
    {
        if(hostToggle.isOn)
        {
            JoinSettings.joinAsClient = false;
            JoinSettings.joinAsHost = true;
            sessionSelect.SetActive(false);
        }

        else
        {
            JoinSettings.joinAsHost = false;
            JoinSettings.joinAsClient = true;
            sessionSelect.SetActive(true);
        }
    }

    public void SetRoomName()
    {
        JoinSettings.roomName = roomName.text;
    }
}
