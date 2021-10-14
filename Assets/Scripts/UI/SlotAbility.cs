using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotAbility : MonoBehaviour
{
    public GameObject ability;

    public GameObject slotButton;

    public GameObject slotPanel;

    public DungeonMasterController controller;

    public float size;

    private Slots slots;

    private int slotCost;

    public void Start()
    {
        slots = slotPanel.GetComponent<Slots>();
        slotCost = ability.GetComponent<EnemyGeneric>().slotSize;
    }

    public void AddToSlot()
    {
        if(slots.available < slotCost)
        {
            Debug.Log("Not Enough Slots");
            return;
        }

        var button = Instantiate(slotButton, slotPanel.transform);
        button.GetComponent<UseAbility>().controller = controller;
        button.GetComponent<UseAbility>().ability = ability;
        button.GetComponent<UseAbility>().slots = slots;
        slots.available -= slotCost;
    }
}
