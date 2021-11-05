using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public GameObject obj;

    public ObjectPlacer objectPlacer;

    public bool isEnemy;

    public void Start()
    {
        transform.GetComponentInChildren<Text>().text = obj.name;
    }

    public void SetCurrentObject()
    {
        objectPlacer.objectToPlace = obj;

        if(isEnemy)
        {
            objectPlacer.enemyPlaceMode = true;
        }

        else
        {
            objectPlacer.enemyPlaceMode = false;
        }
    }
}
