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
}
