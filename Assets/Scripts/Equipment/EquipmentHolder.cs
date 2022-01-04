using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[OrderAfter(typeof(Health))]
[OrderBefore(typeof(PlayerMovement))]
public class EquipmentHolder : NetworkBehaviour
{
    public List<GameObject> equipmentPrefabs = new List<GameObject>();

    private List<Equipment> equipment = new List<Equipment>();

    public override void Spawned()
    {
        SpawnEquipment();

        if(Object.HasStateAuthority)
        {
            ActivateEquipment();
        }
    }

    private void SpawnEquipment()
    {
        foreach (GameObject equip in equipmentPrefabs)
        {
            var e = Instantiate(equip, transform.position, Quaternion.identity, transform);
            Equipment eq;
            e.TryGetComponent(out eq);

            if (eq != null)
            {
                equipment.Add(eq);
            }
        }
    }

    private void ActivateEquipment()
    {
        foreach(Equipment e in equipment)
        {
            e.Activate();
        }
    }
}
