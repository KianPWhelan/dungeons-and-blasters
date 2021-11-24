using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerWeaponController : NetworkBehaviour
{
    private WeaponHolder weaponHolder;

    public override void Spawned()
    {
        weaponHolder = GetComponent<WeaponHolder>();
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out PlayerInput input))
        {
            if(input.IsDown(PlayerInput.BUTTON_FIRE))
            {
                weaponHolder.UseWeapon(0);
            }
        }
    }
}
