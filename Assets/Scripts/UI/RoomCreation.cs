using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Room;

public class RoomCreation : MonoBehaviour
{
    [SerializeField]
    private Map mapGen;

    [SerializeField]
    private MapGridGenerator grid;

    [SerializeField]
    private MapSelect mapSelector;

    [SerializeField]
    private InputField mapNameInput;

    public Vector2Int currentGridLocation;

    [SerializeField]
    public Text gridLocation;

    [SerializeField]
    private Dropdown roomSelector;

    private GameObject selectedRoom;

    [SerializeField]
    private Dropdown slotOptionsSelector;

    private SlotOption selectedSlotOption;

    [SerializeField]
    private Dropdown slotSelector;

    private EnemySlot selectedSlot;

    [SerializeField]
    private Dropdown enemySelector;

    private GameObject selectedEnemy;

    private GameObject[] allAssets;

    private List<GameObject> roomPrefabs = new List<GameObject>();

    private List<GameObject> enemyPrefabs = new List<GameObject>();

    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();

    private Dictionary<SizeClasses, List<GameObject>> sortedEnemies = new Dictionary<SizeClasses, List<GameObject>>();
    // Start is called before the first frame update
    void Start()
    {
        LoadAllResources();
        LoadAllRooms();
        LoadAllEnemies();
        SortEnemiesIntoSizes();
        PopulateRoomSelection();
    }

    public void PopulateRoomSelection()
    {
        roomSelector.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(GameObject room in roomPrefabs)
        {
            options.Add(new Dropdown.OptionData(room.name));
        }

        roomSelector.AddOptions(options);
        UpdateRoomSelection();
    }

    public void UpdateRoomSelection()
    {
        selectedRoom = roomPrefabs[roomSelector.value];

        // Populate cascade
        PopulateSlotOptions();
    }

    public void PopulateSlotOptions()
    {
        slotOptionsSelector.ClearOptions();
        var info = selectedRoom.GetComponent<Room>();

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(SlotOption option in info.slotOptions)
        {
            var sizeData = option.GetNumberOfSlotsPerSize();
            string newOp = "";

            foreach(SizeClasses s in sizeData.Keys)
            {
                newOp += s + " X" + sizeData[s] + "\n";
            }

            options.Add(new Dropdown.OptionData(newOp));
        }

        slotOptionsSelector.AddOptions(options);
        UpdateSlotOptionSelection();
    }

    public void UpdateSlotOptionSelection()
    {
        if(selectedRoom.GetComponent<Room>().slotOptions.Count <= 0)
        {
            slotSelector.ClearOptions();
            enemySelector.ClearOptions();
            return;
        }

        selectedSlotOption = selectedRoom.GetComponent<Room>().slotOptions[slotOptionsSelector.value];
        SetDefaultEnemies();

        // Populate cascade
        PopulateSlotSelection();
    }

    public void PopulateSlotSelection()
    {
        slotSelector.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(EnemySlot slot in selectedSlotOption.slots)
        {
            options.Add(new Dropdown.OptionData(slot.size.ToString()));
        }

        slotSelector.AddOptions(options);
        UpdateSlotSelection();
    }

    public void UpdateSlotSelection()
    {
        selectedSlot = selectedSlotOption.slots[slotSelector.value];

        // Populate cascade
        PopulateEnemySelection();
    }

    public void PopulateEnemySelection()
    {
        enemySelector.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(GameObject enemy in sortedEnemies[selectedSlot.size])
        {
            options.Add(new Dropdown.OptionData(enemy.name));
        }

        enemySelector.AddOptions(options);
        UpdateEnemySelection();
    }   
    
    public void UpdateEnemySelection()
    {
        selectedEnemy = sortedEnemies[selectedSlot.size][enemySelector.value];
        enemies[slotSelector.value] = selectedEnemy;
    }

    public void SaveRoom()
    {
        mapGen.AddRoom(selectedRoom, currentGridLocation, slotOptionsSelector.value, enemies);
        grid.grid[currentGridLocation.x, currentGridLocation.y].GetComponentInChildren<Text>().text = selectedRoom.name + "\n" + currentGridLocation;
    }

    public void SaveMap()
    {
        mapSelector.AddMap(JSONTools.SaveMapData(mapGen.rooms, mapGen.mapSize, mapNameInput.text));
    }

    public void ClearRoom()
    {
        grid.grid[currentGridLocation.x, currentGridLocation.y].GetComponentInChildren<Text>().text = currentGridLocation.ToString();
        mapGen.RemoveRoom(currentGridLocation);
    }

    private void SetDefaultEnemies()
    {
        enemies = new List<GameObject>();

        foreach(EnemySlot slot in selectedSlotOption.slots)
        {
            enemies.Add(sortedEnemies[slot.size][0]);
        }
    }

    private void LoadAllResources()
    {
        allAssets = Resources.LoadAll<GameObject>("");
    }

    private void LoadAllRooms()
    {
        foreach(GameObject obj in allAssets)
        {
            if(obj.tag == "Room")
            {
                roomPrefabs.Add(obj);
            }
        }
    }

    private void LoadAllEnemies()
    {
        foreach(GameObject obj in allAssets)
        {
            if (obj.TryGetComponent(out EnemyGeneric e) && e.slottable)
            {
                enemyPrefabs.Add(obj);
            }
        }
    }

    private void SortEnemiesIntoSizes()
    {
        foreach(GameObject enemy in enemyPrefabs)
        {
            EnemyGeneric e = enemy.GetComponent<EnemyGeneric>();

            if(!sortedEnemies.ContainsKey(e.size))
            {
                List<GameObject> newList = new List<GameObject>();
                newList.Add(enemy);
                sortedEnemies.Add(e.size, newList);
            }

            else
            {
                sortedEnemies[e.size].Add(enemy);
            }
        }
    }
}
