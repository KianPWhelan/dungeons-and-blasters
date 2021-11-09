using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RoomGenerator : MonoBehaviour
{
    public Vector2Int gridSize;

    public float offset = 4;

    public Node[,] nodes;

    public GameObject tilePrefab;

    public GameObject wallPrefab;

    public string name = "testmap";

    public Database database;

    private Dictionary<GameObject, List<GameObject>> batches = new Dictionary<GameObject, List<GameObject>>();

    public bool save;

    public InputField nameText;

    public AutoBake baker;

    public bool genOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        if(!genOnStart)
        {
            return;
        }

        GenerateGrid();
        GenerateWalls();
    }

    // Update is called once per frame
    void Update()
    {
        if(nameText != null)
        {
            name = nameText.text;
        }
    }

    public void SaveRoom()
    {
        string res = JSONTools.SaveRoomData(this);
        File.WriteAllText("room.txt", res);
        database.SaveRoomUnderCurrentUser(res, name);
    }

    public async void LoadRoom()
    {
        StringHolder json = new StringHolder();
        await database.LoadRoomFromCurrentUserByName(name, json);
        Debug.Log("Finished load db data");
        LoadRoomFromJson(json.value);
    }

    public void LoadRoomFromJson(string roomJson)
    {
        Debug.Log(roomJson);

        int childs = transform.childCount;

        for(int i = 0; i < childs; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        var roomData = JSONTools.LoadRoomDataFromJson(roomJson);

        gridSize = roomData.gridSize;
        name = roomData.name;

        GenerateGrid();
        GenerateWalls();

        foreach(NodeData node in roomData.nodes)
        {
            if(node.isObjOrigin)
            {
                AddObjectToNode(node.gridLocation, Resources.Load<GameObject>("Objects/" + node.obj), node.objOrientation);
            }
        }

        foreach(NodeData node in roomData.nodes)
        {
            if(node.enemy != "empty")
            {
                AddEnemyToNode(node.gridLocation, Resources.Load<GameObject>("RoomPlaceholders/" + node.enemy));
            }
        }

        baker.BakeAll();
    }

    public void SpawnAllEnemies()
    {
        for(int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0; j < gridSize.y; j++)
            {
                if(nodes[i, j].enemy != null)
                {
                    nodes[i, j].enemy.GetComponent<RoomPlaceholder>().SpawnEnemies();
                }
            }
        }
    }

    public void GenerateGrid()
    {
        nodes = new Node[gridSize.x, gridSize.y];

        for(int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0; j < gridSize.y; j++)
            {
                var newTile = Instantiate(tilePrefab, new Vector3(i * offset, 0, j * offset), tilePrefab.transform.rotation, transform);
                newTile.isStatic = true;
                Node node = new Node();
                // Add data to node
                node.tile = newTile;
                node.gridLocation = new Vector2Int(i, j);
                nodes[i, j] = node;
            }
        }
    }

    public void GenerateWalls()
    {
        // Generate along top and bottom
        for (int i = 0; i < gridSize.x; i++)
        {
            var newWallTop = Instantiate(wallPrefab, new Vector3(i * offset, 0, (gridSize.y * offset) - (offset / 2)), Quaternion.identity, transform);
            var newWallBot = Instantiate(wallPrefab, new Vector3(i * offset, 0, -offset / 2), Quaternion.identity, transform);
            //newWallTop.isStatic = true;
            //newWallTop.transform.GetChild(0).gameObject.isStatic = true;
            //newWallBot.isStatic = true;
            //newWallBot.transform.GetChild(0).gameObject.isStatic = true;
        }

        // Generate along sides
        for(int i = 0; i < gridSize.y; i++)
        {
            var newWallLeft = Instantiate(wallPrefab, new Vector3(-offset / 2, 0, i * offset), Quaternion.FromToRotation(Vector3.forward, Vector3.left), transform);
            var newWallRight = Instantiate(wallPrefab, new Vector3((gridSize.x * offset) - (offset / 2), 0, i * offset), Quaternion.FromToRotation(Vector3.forward, Vector3.left), transform);
            //newWallLeft.isStatic = true;
            //newWallLeft.transform.GetChild(0).gameObject.isStatic = true;
            //newWallRight.isStatic = true;
            //newWallRight.transform.GetChild(0).gameObject.isStatic = true;
        }
    }

    public Vector3 GetStartPoint()
    {
        for(int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0; j < gridSize.y; j++)
            {
                if(nodes[i, j].isObjOrigin && nodes[i, j].obj.GetComponent<RoomObject>().isStartingPoint)
                {
                    return nodes[i, j].tile.transform.position;
                }
            }
        }

        return Vector3.zero;
    }

    public void AddEnemyToNode(GameObject tile, GameObject enemy, Vector3 position)
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                if (nodes[i, j].tile == tile && nodes[i, j].enemy == null)
                {
                    Debug.Log("Found tile at node " + i + " " + j);
                    var newEnemy = Instantiate(enemy, position, Quaternion.identity, transform);

                    if(nodes[i, j].obj != null)
                    {
                        RelocateEnemyToCorrectHeight(nodes[i, j], newEnemy);
                    }

                    nodes[i, j].enemy = newEnemy;
                }
            }
        }
    }

    public void AddEnemyToNode(Vector2Int gridLocation, GameObject enemy)
    {
        var newEnemy = Instantiate(enemy, nodes[gridLocation.x, gridLocation.y].tile.transform.position, Quaternion.identity, transform);

        if (nodes[gridLocation.x, gridLocation.y].obj != null)
        {
            RelocateEnemyToCorrectHeight(nodes[gridLocation.x, gridLocation.y], newEnemy);
        }

        nodes[gridLocation.x, gridLocation.y].enemy = newEnemy;
    }

    public void AddObjectToNode(GameObject tile, GameObject obj, Vector3 position, Vector3 rotation)
    {
        for(int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0; j < gridSize.y; j++)
            {
                if(nodes[i, j].tile == tile)
                {
                    Debug.Log("Found tile at node " + i + " " + j);
                    SetObjectOrientationAndOccuption(new Vector2Int(i, j), obj, position, rotation);
                }
            }
        }
    }

    public void AddObjectToNode(Vector2Int gridLocation, GameObject obj, Vector3 rotation)
    {
        SetObjectOrientationAndOccuption(gridLocation, obj, nodes[gridLocation.x, gridLocation.y].tile.transform.position, rotation);
    }

    public void RemoveObject(GameObject obj)
    {
        List<Node> foundNodes = new List<Node>();
        obj.SetActive(false);

        for(int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0; j < gridSize.y; j++)
            {
                if(nodes[i, j].obj == obj)
                {
                    Debug.Log("Found node with obj at " + i + " " + j);
                    foundNodes.Add(nodes[i, j]);
                }
            }
        }

        foreach(Node node in foundNodes)
        {
            node.obj = null;
            node.isObjOrigin = false;
            node.objOrientation = Vector3.zero;

            if(node.enemy != null)
            {
                RelocateEnemyToCorrectHeight(node, node.enemy);
            }
        }

        Destroy(obj);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        for(int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0; j < gridSize.y; j++)
            {
                if(nodes[i, j].enemy == enemy)
                {
                    nodes[i, j].enemy = null;
                    Destroy(enemy);
                }
            }
        }
    }

    private void RelocateEnemyToCorrectHeight(Node node, GameObject enemy)
    {
        RaycastHit hit;

        if(Physics.Raycast(node.tile.transform.position + (Vector3.up * 50), Vector3.down, out hit, 1000))
        {
            enemy.transform.position = hit.point;
        }
    }

    private void SetObjectOrientationAndOccuption(Vector2Int startSpot, GameObject obj, Vector3 position, Vector3 rotation)
    {
        int x = startSpot.x;
        int y = startSpot.y;

        var objInfo = obj.GetComponent<RoomObject>();

        List<Vector2Int> nodesToFill = new List<Vector2Int>();

        Vector2Int occupation = Orientator(objInfo.tilesOccupied, rotation);

        if(occupation.x < 0)
        {
            x += occupation.x + 1;
        }

        if(occupation.y < 0)
        {
            y += occupation.y + 1;
        }

        for(int i = x; i < x + Mathf.Abs(occupation.x); i++)
        {
            for(int j = y; j < y + Mathf.Abs(occupation.y); j++)
            {
                Debug.Log(i + " " + j);
                if(nodes[i, j].obj == null)
                {
                    Debug.Log("Occupying tile at " + i + " " + j);
                    nodesToFill.Add(new Vector2Int(i, j));
                }

                else
                {
                    return;
                }
            }
        }

        nodes[startSpot.x, startSpot.y].isObjOrigin = true;
        nodes[startSpot.x, startSpot.y].objOrientation = rotation;

        var newObj = Instantiate(obj, position, Quaternion.LookRotation(rotation, Vector3.up), transform);
        //newObj.isStatic = true;
        //newObj.transform.GetChild(0).gameObject.isStatic = true;

        foreach (Vector2Int loc in nodesToFill)
        {
            nodes[loc.x, loc.y].obj = newObj;
        }

        // Batch(obj, newObj);
        //StaticBatchingUtility.Combine(gameObject);
    }

    private Vector2Int Orientator(Vector2Int tilesOccupied, Vector3 rotation)
    {
        if(rotation == Vector3.right)
        {
            return new Vector2Int(tilesOccupied.y, -tilesOccupied.x);
        }

        if(rotation == -Vector3.forward)
        {
            return new Vector2Int(-tilesOccupied.x, -tilesOccupied.y);
        }

        if(rotation == Vector3.left)
        {
            return new Vector2Int(-tilesOccupied.y, tilesOccupied.x);
        }

        return tilesOccupied;
    }

    private void Batch(GameObject prefab, GameObject newObj)
    {
        if(batches.ContainsKey(prefab))
        {
            batches[prefab].Add(newObj.transform.GetChild(0).gameObject);
        }

        else
        {
            List<GameObject> newList = new List<GameObject>();
            newList.Add(newObj.transform.GetChild(0).gameObject);
            batches.Add(prefab, newList);
        }

        StaticBatchingUtility.Combine(batches[prefab].ToArray(), gameObject);
    }
}