using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPlacer : MonoBehaviour
{
    public Camera cam;

    public GameObject objectToPlace;

    public RoomGenerator grid;

    public bool enemyPlaceMode;

    public GameObject objectButtonPrefab;

    public GameObject objectButtonContentSection;

    private Vector3[] rotations = new Vector3[4];

    [SerializeField]
    private int currentRotation = 0;

    private List<GameObject> enemyPrefabs = new List<GameObject>();

    private List<GameObject> objectPrefabs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        rotations[0] = Vector3.forward;
        rotations[1] = Vector3.right;
        rotations[2] = -Vector3.forward;
        rotations[3] = Vector3.left;
        LoadObjectSelectors();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlaceObjectAtNode();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            RemoveObjectAtNode();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            currentRotation++;

            if(currentRotation > 3)
            {
                currentRotation = 0;
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            currentRotation--;

            if(currentRotation < 0)
            {
                currentRotation = 3;
            }
        }
    }

    public void PlaceObjectAtNode()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // we're over a UI element... peace out
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;

        if (!enemyPlaceMode && Physics.Raycast(ray, out h, 1000))
        {
            if(!enemyPlaceMode && h.collider.tag == "Ground")
            {
                grid.AddObjectToNode(h.collider.gameObject, objectToPlace, h.transform.position, rotations[currentRotation]);
            }

            //else if(enemyPlaceMode && hit.collider.tag == "Ground")
            //{
            //    bool isObject = false;

            //    if(hit.transform.parent.TryGetComponent(out RoomObject r))
            //    {
            //        Debug.Log("Is object");
            //        isObject = true;
            //        Debug.Log(hit.collider.transform.parent.gameObject);
            //        grid.AddEnemyToNode(hit.collider.transform.parent.gameObject, objectToPlace, hit.transform.position, isObject);
            //    }

            //    grid.AddEnemyToNode(hit.collider.gameObject, objectToPlace, hit.transform.position, isObject);
            //}
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(ray, 1000);

        foreach(RaycastHit hit in hits)
        {
            if(hit.collider.tag == "Ground" && !hit.collider.transform.parent.TryGetComponent(out RoomObject r))
            {
                grid.AddEnemyToNode(hit.collider.gameObject, objectToPlace, hit.transform.position);
            }
        }
    }

    public void RemoveObjectAtNode()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // we're over a UI element... peace out
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;

        if (!enemyPlaceMode && Physics.Raycast(ray, out h, 1000))
        {
            if (!enemyPlaceMode && h.collider.tag == "Ground" && h.collider.transform.parent.gameObject.TryGetComponent(out RoomObject r))
            {
                Debug.Log(h.collider.transform.parent.gameObject);
                grid.RemoveObject(h.collider.transform.parent.gameObject);
            }
        }

        else
        {
            if(Physics.Raycast(ray, out h, 1000))
            {
                if(h.collider.gameObject.TryGetComponent(out RoomPlaceholder r))
                {
                    Debug.Log(h.collider.gameObject);
                    grid.RemoveEnemy(h.collider.gameObject);
                }
            }
        }
    }

    private void LoadObjectSelectors()
    {
        LoadObjectPrefabs();

        foreach(GameObject obj in objectPrefabs)
        {
            var button = Instantiate(objectButtonPrefab, objectButtonContentSection.transform);
            var os = button.GetComponent<ObjectSelector>();
            os.objectPlacer = this;
            os.obj = obj;
        }
    }

    private void LoadEnemyPrefabs()
    {
        var prefabs = Resources.LoadAll("", typeof(GameObject));
        // Debug.Log(prefabs.Length);

        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject p = (GameObject)prefabs[i];
            // Debug.Log(p.name);
            if (p.TryGetComponent(out RoomPlaceholder c))
            {
                enemyPrefabs.Add(p);
            }
        }
    }

    private void LoadObjectPrefabs()
    {
        var prefabs = Resources.LoadAll("", typeof(GameObject));
        // Debug.Log(prefabs.Length);

        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject p = (GameObject)prefabs[i];
            // Debug.Log(p.name);
            if (p.TryGetComponent(out RoomObject c))
            {
                objectPrefabs.Add(p);
            }
        }
    }
}
