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
}
