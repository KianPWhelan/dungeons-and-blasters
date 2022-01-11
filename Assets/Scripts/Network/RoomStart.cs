using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RoomStart : MonoBehaviour
{
    private NetworkDebugStart nds;

    // Start is called before the first frame update
    void Start()
    {
        nds = GetComponent<NetworkDebugStart>();
        RunStartup();
    }

    private void RunStartup()
    {
        nds.DefaultRoomName = JoinSettings.roomName;
        Debug.Log("Join as client? " + JoinSettings.joinAsClient);

        if (JoinSettings.joinAsHost)
        {
            nds.StartHost();
        }

        else if(JoinSettings.joinAsClient)
        {
            Debug.Log("Starting client in room " + JoinSettings.roomName);
            nds.StartClient();
        }
    }
}
