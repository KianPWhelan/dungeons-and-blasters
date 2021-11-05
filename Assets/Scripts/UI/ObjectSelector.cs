using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public GameObject obj;

    public ObjectPlacer objectPlacer;

    public void Start()
    {
        transform.GetComponentInChildren<Text>().text = obj.name;
    }

    public void SetCurrentObject()
    {
        objectPlacer.objectToPlace = obj;
    }
}
