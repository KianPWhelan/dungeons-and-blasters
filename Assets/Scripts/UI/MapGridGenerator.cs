using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Map mapGenerator;

    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private RoomCreation roomEditor;

    public Vector2Int currentGridSelection;

    [HideInInspector]
    public GameObject[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[mapGenerator.mapSize.x, mapGenerator.mapSize.y];

        for (int i = 0; i < mapGenerator.mapSize.x; i++)
        {
            for(int j = 0; j < mapGenerator.mapSize.y; j++)
            {
                var button = Instantiate(buttonPrefab, transform);
                button.GetComponent<SelectRoomButton>().gridLocation = new Vector2Int(i, j);
                grid[i, j] = button;
            }
        }

        SetCurrentGridSelection(new Vector2Int(0, 0));
    }

    public void SetCurrentGridSelection(Vector2Int selection)
    {
        currentGridSelection = selection;
        roomEditor.currentGridLocation = currentGridSelection;
        roomEditor.gridLocation.text = "Current Grid Selection: " + currentGridSelection;
    }
}
