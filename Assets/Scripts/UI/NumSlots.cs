using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumSlots : MonoBehaviour
{
    public Slots slots;

    private Text text;

    public void Start()
    {
        text = GetComponent<Text>();
    }

    public void Update()
    {
        text.text = "Slots: " + slots.available;
    }
}
