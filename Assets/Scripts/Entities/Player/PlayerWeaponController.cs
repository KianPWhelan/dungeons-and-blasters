using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerWeaponController : NetworkBehaviour
{
    private WeaponHolder weaponHolder;
    private StatusEffects statusEffects;
    private PlayerMovement playerMovement;

    public void Start()
    {
        if(!Object.HasInputAuthority)
        {
            return;
        }

        if (WeaponSelect.selection != null)
        {
            RPC_AddWeapon(WeaponSelect.selection.name);
        }
    }

    public override void Spawned()
    {
        statusEffects = GetComponent<StatusEffects>();
        weaponHolder = GetComponent<WeaponHolder>();
        playerMovement = GetComponent<PlayerMovement>();
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
                weaponHolder.UseWeapon(0, useRotation: true, rotation: Quaternion.Euler((float) playerMovement.pitch, (float) playerMovement.yaw, 0));
            }

            if (input.IsDown(PlayerInput.BUTTON_FIRE_ALT))
            {
                weaponHolder.UseWeapon(0, altAttack: true, useRotation: true, rotation: Quaternion.Euler((float)playerMovement.pitch, (float)playerMovement.yaw, 0));
            }
        } 
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    private void RPC_AddWeapon(string name)
    {
        weaponHolder.AddWeapon((GameObject)Resources.Load(name), "Enemy");
    }
}
