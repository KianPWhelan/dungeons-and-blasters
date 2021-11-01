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
        public ConnectionPoints points;

        public RoomContainer(GameObject room)
        {
            this.room = room;
            points = room.GetComponent<ConnectionPoints>();
        }
    }

    public void Start()
    {
        rooms = new RoomContainer[mapSize.x, mapSize.y];
        filledRooms = new bool[mapSize.x, mapSize.y];
        AddRoom(roomPrefab, new Vector2Int(0, 0));
        AddRoom(roomPrefab, new Vector2Int(1, 0));
        AddRoom(roomPrefab, new Vector2Int(0, 1));
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
    }

    private void ConnectNeighbors(int x, int y)
    {
        if(!filledRooms[x, y])
        {
            // Instantiate current room
            GenerateRoom(rooms[x, y], x, y);
            filledRooms[x, y] = true;
        }

        // Try Connect North
        if (y + 1 < mapSize.y && rooms[x, y].points.doorPoints[0] != null && !filledRooms[x, y + 1] && rooms[x, y + 1] != null && rooms[x, y + 1].points.doorPoints[1] != null)
        {
            GenerateRoom(rooms[x, y + 1], x, y + 1);
            filledRooms[x, y + 1] = true;
        }

        // Try Connect South
        if (y - 1 >= 0 && rooms[x, y].points.doorPoints[1] != null && !filledRooms[x, y - 1] && rooms[x, y - 1] != null && rooms[x, y - 1].points.doorPoints[0] != null)
        {
            GenerateRoom(rooms[x, y - 1], x, y - 1);
            filledRooms[x, y - 1] = true;
        }

        // Try Connect East
        if(x + 1 < mapSize.x && rooms[x, y].points.doorPoints[2] != null && !filledRooms[x + 1, y] && rooms[x + 1, y] != null && rooms[x + 1, y].points.doorPoints[3] != null)
        {
            GenerateRoom(rooms[x + 1, y], x + 1, y);
            filledRooms[x + 1, y] = true;
        }

        // Try Connect West
        if (x - 1 >= 0 && rooms[x, y].points.doorPoints[3] != null && !filledRooms[x - 1, y] && rooms[x - 1, y] != null && rooms[x - 1, y].points.doorPoints[2] != null)
        {
            GenerateRoom(rooms[x - 1, y], x - 1, y);
            filledRooms[x - 1, y] = true;
        }
    }

    private void GenerateRoom(RoomContainer room, int x, int y)
    {
        Vector3 gridSpot = new Vector3((maxRoomSize.x * x) + (maxRoomSize.x / 2), 0, (maxRoomSize.y * y) + (maxRoomSize.y / 2));

        Instantiate(room.room, gridSpot, Quaternion.identity, transform);
    }
}
