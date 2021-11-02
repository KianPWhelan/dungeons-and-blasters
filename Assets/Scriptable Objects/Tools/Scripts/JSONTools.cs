using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Map;
using static Room;
using Newtonsoft.Json;

public class JSONTools : ScriptableObject
{
    [SerializeField]
    private string fileName;

    public void SaveGameObjectsToJSONTextFile()
    {
        PrefabSaver.SaveEnemyPrefabBalanceData(fileName);
    }

    public void LoadEnemyPrefabBalanceDataFromTextFile()
    {
        PrefabLoader.LoadEnemyPrefabBalanceData(fileName);
    }

    public static string SaveMapData(RoomContainer[,] rooms, Vector2Int mapSize, string name)
    {
        MapData mapData = new MapData();
        mapData.map = new RoomData[mapSize.x, mapSize.y];
        mapData.mapSize = mapSize;
        mapData.name = name;

        for(int i = 0; i < mapSize.x; i++)
        {
            for(int j = 0; j < mapSize.y; j++)
            {
                if (rooms[i, j] != null)
                {
                    mapData.map[i, j] = new RoomData(rooms[i, j].room.name, rooms[i, j].slotChoice, GetEnemyNames(rooms[i, j].enemies));
                }

                else
                {
                    mapData.map[i, j] = new RoomData("empty", 0, new string[0]);
                }
            }
        }

        string mapJson = JsonConvert.SerializeObject(mapData);

        Debug.Log(mapJson);

        return mapJson;
    }

    public static MapData LoadMapData(string jsonMap)
    {
        MapData mapData = JsonConvert.DeserializeObject<MapData>(jsonMap);

        return mapData;
    }

    private static string[] GetEnemyNames(List<GameObject> enemies)
    {
        string[] names = new string[enemies.Count];
        int i = 0;

        foreach(GameObject enemy in enemies)
        {
            names[i] = enemy.name;
            i++;
        }

        return names;
    }
}
