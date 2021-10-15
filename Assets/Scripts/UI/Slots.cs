using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public int numSlots;
    public int available;

    public void Start()
    {
        available = numSlots;
    }

    public void SetSlots()
    {
        var playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        numSlots += playerCount - 1;
        available += playerCount - 1;
    }
}
