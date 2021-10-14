using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UseAbility : MonoBehaviour, IPointerClickHandler
{
    public GameObject ability;

    public DungeonMasterController controller;

    public Slots slots;

    public int size;

    private Text text;

    private float cooldownTime;

    private float cooldown;

    // Start is called before the first frame update
    public void Start()
    {
        var data = ability.GetComponent<EnemyGeneric>();
        cooldownTime = data.cooldown;
        cooldown = cooldownTime;
        size = data.slotSize;
        text = GetComponentInChildren<Text>();
        text.text = ability.name + "\nCooldown: " + cooldown.ToString("0.00"); ;
    }
    public void Update()
    {
        cooldown -= Time.deltaTime;
        text.text = ability.name + "\nCooldown: " + cooldown.ToString("0.00"); ;

        if(cooldown <= 0)
        {
            text.text = ability.name + "\nReady";
        }
    }

    public void SetCurrentAbility()
    {
        if(cooldown <= 0)
        {
            controller.SetCurrentSelection(ability);
            cooldown = cooldownTime;
        }      
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            slots.available += size;
            Destroy(gameObject);
        }
    }
}
