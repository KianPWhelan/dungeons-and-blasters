using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectRoomButton : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int gridLocation;

    public void Start()
    {
        GetComponentInChildren<Text>().text = gridLocation.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.parent.GetComponent<MapGridGenerator>().SetCurrentGridSelection(gridLocation);
    }
}
