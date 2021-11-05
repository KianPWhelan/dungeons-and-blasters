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
        mapData.map = new RoomDataDeprecated[mapSize.x, mapSize.y];
        mapData.mapSize = mapSize;
        mapData.name = name;

        for(int i = 0; i < mapSize.x; i++)
        {
            for(int j = 0; j < mapSize.y; j++)
            {
                if (rooms[i, j] != null)
                {
                    mapData.map[i, j] = new RoomDataDeprecated(rooms[i, j].room.name, rooms[i, j].slotChoice, GetEnemyNames(rooms[i, j].enemies));
                }

                else
                {
                    mapData.map[i, j] = new RoomDataDeprecated("empty", 0, new string[0]);
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

    public static string SaveRoomData(RoomGenerator room)
    {
        RoomData roomData = new RoomData();

        roomData.name = room.name;
        roomData.gridSize = room.gridSize;
        roomData.offset = room.offset;

        roomData.nodes = new NodeData[room.gridSize.x * room.gridSize.y];

        int count = 0;

        for(int i = 0; i < room.gridSize.x; i++)
        {
            for(int j = 0; j < room.gridSize.y; j++)
            {
                Node node = room.nodes[i, j];

                string objName = "empty";
                string enemyName = "empty";
                bool isObjOrigin = false;
                Vector3 objOrientation = Vector3.zero;

                if(node.obj != null && node.isObjOrigin)
                {
                    objName = node.obj.name;
                    int index = objName.IndexOf("(Clone)");
                    objName = (index < 0)
                        ? objName
                        : objName.Remove(index, "(Clone)".Length);
                    isObjOrigin = true;
                    objOrientation = node.objOrientation;
                }

                if(node.enemy != null)
                {
                    enemyName = (string)node.enemy.name.Clone();
                    int index = enemyName.IndexOf("(Clone)");
                    enemyName = (index < 0)
                        ? enemyName
                        : enemyName.Remove(index, "(Clone)".Length);
                }

                roomData.nodes[count] = new NodeData(node.gridLocation, objName, isObjOrigin, objOrientation, enemyName);
                count++;
            }
        }

        string roomJson = JsonConvert.SerializeObject(roomData);

        Debug.Log(roomJson);
        return roomJson;
    }

    public static RoomData LoadRoomDataFromJson(string roomJson)
    {
        return JsonConvert.DeserializeObject<RoomData>(roomJson);
    }
}
