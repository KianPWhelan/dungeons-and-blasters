using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public RoomContainer[,] rooms;

    public Vector2Int mapSize = new Vector2Int(5, 5);

    public Vector2 maxRoomSize = new Vector2();

    public GameObject roomPrefab;

    private bool[,] filledRooms;

    public class RoomContainer
    {
        public GameObject room;
        public Room info;

        public RoomContainer(GameObject room)
        {
            this.room = room;
            info = room.GetComponent<Room>();
        }
    }

    public void Start()
    {
        rooms = new RoomContainer[mapSize.x, mapSize.y];
        filledRooms = new bool[mapSize.x, mapSize.y];
        AddRoom(roomPrefab, new Vector2Int(0, 0));
        AddRoom(roomPrefab, new Vector2Int(1, 0));
        AddRoom(roomPrefab, new Vector2Int(0, 1));
        //rooms[0, 0].info.selection = rooms[0, 0].info.slotOptions[1];
        //rooms[0, 0].info.PlaceEnemyInSlot(Resources.Load<GameObject>("Ogre"), 1);
        //string mapJSon = JSONTools.SaveMapData(rooms, mapSize);
        //LoadMapFromJson(mapJSon);
        BuildMap();
        rooms[0, 0].info.ActivateRoom();
    }

    public void LoadMapFromJson(string mapJson)
    {
        MapData mapData = JSONTools.LoadMapData(mapJson);
        mapSize = mapData.mapSize;
        rooms = new RoomContainer[mapSize.x, mapSize.y];
        filledRooms = new bool[mapSize.x, mapSize.y];

        for (int i = 0; i < mapSize.x; i++)
        {
            for(int j = 0; j < mapSize.y; j++)
            {
                if(mapData.map[i, j].prefabName != "empty")
                {
                    AddRoom(Resources.Load<GameObject>(mapData.map[i, j].prefabName), new Vector2Int(i, j));
                }
            }
        }

        BuildMap();
    }

    public void AddRoom(GameObject room, Vector2Int location)
    {
        rooms[location.x, location.y] = new RoomContainer(room);
    }

    public void RemoveRoom(Vector2Int location)
    {
        rooms[location.x, location.y] = null;
    }

    public void BuildMap()
    {
        for(int i = 0; i < mapSize.x; i++)
        {
            for(int j = 0; j < mapSize.y; j++)
            {
                if(rooms[i, j] != null)
                {
                    GenerateRoom(rooms[i, j], i, j);
                }
            }
        }

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (rooms[i, j] != null)
                {
                    ConnectNeighbors(i, j);
                }
            }
        }
    }

    private void ConnectNeighbors(int x, int y)
    {
        // Try Connect North
        if (y + 1 < mapSize.y && rooms[x, y].info.doorPoints[0] != null && rooms[x, y + 1] != null && rooms[x, y + 1].info.doorPoints[1] != null)
        {
            rooms[x, y].info.doorPoints[0].GetComponent<Teleporter>().target = rooms[x, y + 1].info.doorPoints[1];
        }

        // Try Connect South
        if (y - 1 >= 0 && rooms[x, y].info.doorPoints[1] != null && rooms[x, y - 1] != null && rooms[x, y - 1].info.doorPoints[0] != null)
        {
            rooms[x, y].info.doorPoints[1].GetComponent<Teleporter>().target = rooms[x, y - 1].info.doorPoints[0];
        }

        // Try Connect East
        if (x + 1 < mapSize.x && rooms[x, y].info.doorPoints[2] != null && rooms[x + 1, y] != null && rooms[x + 1, y].info.doorPoints[3] != null)
        {
            rooms[x, y].info.doorPoints[2].GetComponent<Teleporter>().target = rooms[x + 1, y].info.doorPoints[3];
        }

        // Try Connect West
        if (x - 1 >= 0 && rooms[x, y].info.doorPoints[3] != null && rooms[x - 1, y] != null && rooms[x - 1, y].info.doorPoints[2] != null)
        {
            rooms[x, y].info.doorPoints[3].GetComponent<Teleporter>().target = rooms[x - 1, y].info.doorPoints[2];
        }
    }

    private void GenerateRoom(RoomContainer room, int x, int y)
    {
        Vector3 gridSpot = new Vector3((maxRoomSize.x * x) + (maxRoomSize.x / 2), 0, (maxRoomSize.y * y) + (maxRoomSize.y / 2));

        var newRoom = Instantiate(room.room, gridSpot, Quaternion.identity, transform);
        room.room = newRoom;
        room.info = newRoom.GetComponent<Room>();
    }
}
