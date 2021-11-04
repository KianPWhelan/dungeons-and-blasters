using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public Camera cam;

    public GameObject objectToPlace;

    public GridGenerator grid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlaceObjectAtNode();
        }
    }

    public void PlaceObjectAtNode()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            if(hit.collider.tag == "Ground")
            {
                grid.AddObjectToNode(hit.collider.gameObject, objectToPlace, hit.transform.position);
            }
        }
    }
}
