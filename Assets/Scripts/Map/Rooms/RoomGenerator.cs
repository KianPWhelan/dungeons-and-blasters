using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public Vector2Int gridSize;

    public float offset = 4;

    public Node[,] nodes;

    public GameObject tilePrefab;

    public GameObject wallPrefab;

    private Dictionary<GameObject, List<GameObject>> batches = new Dictionary<GameObject, List<GameObject>>();

    // Start is called before the first frame update
    void Awake()
    {
        GenerateGrid();
        GenerateWalls();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            newWallTop.isStatic = true;
            newWallTop.transform.GetChild(0).gameObject.isStatic = true;
            newWallBot.isStatic = true;
            newWallBot.transform.GetChild(0).gameObject.isStatic = true;
        }

        // Generate along sides
        for(int i = 0; i < gridSize.y; i++)
        {
            var newWallLeft = Instantiate(wallPrefab, new Vector3(-offset / 2, 0, i * offset), Quaternion.FromToRotation(Vector3.forward, Vector3.left), transform);
            var newWallRight = Instantiate(wallPrefab, new Vector3((gridSize.x * offset) - (offset / 2), 0, i * offset), Quaternion.FromToRotation(Vector3.forward, Vector3.left), transform);
            newWallLeft.isStatic = true;
            newWallLeft.transform.GetChild(0).gameObject.isStatic = true;
            newWallRight.isStatic = true;
            newWallRight.transform.GetChild(0).gameObject.isStatic = true;
        }
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
                        PlaceEnemyOnTopOfObject(nodes[i, j], newEnemy);
                    }

                    nodes[i, j].enemy = newEnemy;
                }
            }
        }
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

    private void PlaceEnemyOnTopOfObject(Node node, GameObject enemy)
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

        foreach(Vector2Int loc in nodesToFill)
        {
            nodes[loc.x, loc.y].obj = obj;
        }

        var newObj = Instantiate(obj, position, Quaternion.LookRotation(rotation, Vector3.up), transform);
        newObj.isStatic = true;
        newObj.transform.GetChild(0).gameObject.isStatic = true;
        // Batch(obj, newObj);
        StaticBatchingUtility.Combine(gameObject);
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
