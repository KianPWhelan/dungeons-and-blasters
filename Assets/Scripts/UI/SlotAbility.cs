using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotAbility : MonoBehaviour
{
    public GameObject ability;

    public GameObject slotButton;

    public GameObject slotPanel;

    public DungeonMasterController controller;

    public float size;

    private UseAbility buttonInstance;

    private Slots slots;

    private int slotCost;

    private int charges;

    private Text buttonText;

    private EnemyGeneric data;

    public void Start()
    {
        slots = slotPanel.GetComponent<Slots>();
        data = ability.GetComponent<EnemyGeneric>();
        slotCost = data.slotSize;
        buttonText = GetComponentInChildren<Text>();
    }

    public void AddToSlot()
    {
        if(slots.available < slotCost)
        {
            Debug.Log("Not Enough Slots");
            return;
        }

        if(buttonInstance == null)
        {
            var button = Instantiate(slotButton, slotPanel.transform);
            buttonInstance = button.GetComponent<UseAbility>();
            buttonInstance.controller = controller;
            buttonInstance.ability = ability;
            buttonInstance.slots = slots;
            buttonInstance.amount = 1;
        }

        else
        {
            buttonInstance.amount += 1;
            buttonInstance.cooldownTime = buttonInstance.startingCooldownTime / buttonInstance.amount;

            if(buttonInstance.cooldown > buttonInstance.cooldownTime)
            {
                buttonInstance.cooldown = buttonInstance.cooldownTime;
            }
        }
        
        slots.available -= slotCost;
    }
}
