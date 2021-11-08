using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadRooms : MonoBehaviour
{
    public Database database;
    public RoomSelect selection;

    public async void LoadAllRooms()
    {
        List<string> rooms = new List<string>();

        await database.LoadAllRoomsForCurrentUser(rooms);

        foreach (string s in rooms)
        {
            Debug.Log(s);
            selection.AddMap(s);
        }
    }
}
