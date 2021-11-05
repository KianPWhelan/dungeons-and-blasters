using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public Camera cam;

    public GameObject objectToPlace;

    public RoomGenerator grid;

    public bool enemyPlaceMode;

    private Vector3[] rotations = new Vector3[4];

    [SerializeField]
    private int currentRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        rotations[0] = Vector3.forward;
        rotations[1] = Vector3.right;
        rotations[2] = -Vector3.forward;
        rotations[3] = Vector3.left;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlaceObjectAtNode();
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
}
