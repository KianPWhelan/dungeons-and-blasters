using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerWeaponController : NetworkBehaviour
{
    private WeaponHolder weaponHolder;
    private StatusEffects statusEffects;

    public override void Spawned()
    {
        statusEffects = GetComponent<StatusEffects>();
        weaponHolder = GetComponent<WeaponHolder>();

        if(WeaponSelect.selection != null)
        {
            weaponHolder.AddWeapon(WeaponSelect.selection, "Enemy");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(statusEffects.GetIsStunned())
        {
            return;
        }

        if(GetInput(out PlayerInput input))
        {
            if(input.IsDown(PlayerInput.BUTTON_FIRE))
            {
                weaponHolder.UseWeapon(0);
            }
        }

        if (input.IsDown(PlayerInput.BUTTON_FIRE_ALT))
        {
            weaponHolder.UseWeapon(0, altAttack: true);
        }
    }
}
