using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Vector2Int gridSize;

    public float offset = 4;

    public Node[,] nodes;

    public GameObject tilePrefab;

    public GameObject wallPrefab;

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
        }

        // Generate along sides
        for(int i = 0; i < gridSize.y; i++)
        {
            var newWallLeft = Instantiate(wallPrefab, new Vector3(-offset / 2, 0, i * offset), Quaternion.FromToRotation(Vector3.forward, Vector3.left), transform);
            var newWallRight = Instantiate(wallPrefab, new Vector3((gridSize.x * offset) - (offset / 2), 0, i * offset), Quaternion.FromToRotation(Vector3.forward, Vector3.left), transform);
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
}
