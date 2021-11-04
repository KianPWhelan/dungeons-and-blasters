using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public Camera cam;

    public GameObject objectToPlace;

    public GridGenerator grid;

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
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            if(hit.collider.tag == "Ground")
            {
                grid.AddObjectToNode(hit.collider.gameObject, objectToPlace, hit.transform.position, rotations[currentRotation]);
            }
        }
    }
}
