using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[OrderBefore(typeof(HitboxManager))]
public class PlayerMovement : NetworkBehaviour
{
    private NetworkCharacterController cc;

    [Networked]
    public Angle yaw { get; set; }

    [Networked]
    public Angle pitch { get; set; }

    [HideInInspector]
    public FPSCamera cam;

    private StatusEffects statusEffects;
    private float startingSpeed;

    [HideInInspector]
    public static NetworkObject localPlayer;

    public override void Spawned()
    {
        cc = GetComponent<NetworkCharacterController>();
        statusEffects = GetComponent<StatusEffects>();
        startingSpeed = cc.MaxSpeed;
        SetupCamera();
    }

    public override void FixedUpdateNetwork()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        Vector3 direction = default;

        if (GetInput(out PlayerInput input))
        {
            if (input.IsDown(PlayerInput.BUTTON_FORWARD))
            {
                direction += transform.forward;
            }

            if (input.IsDown(PlayerInput.BUTTON_BACKWARDS))
            {
                direction -= transform.forward;
            }

            if (input.IsDown(PlayerInput.BUTTON_LEFT))
            {
                direction -= transform.right;
            }

            if (input.IsDown(PlayerInput.BUTTON_RIGHT))
            {
                direction += transform.right;
            }

            if (input.IsDown(PlayerInput.BUTTON_JUMP))
            {
                cc.Jump();
            }

            yaw += input.yaw;
            pitch += input.pitch;
        }

        if(statusEffects.GetIsStunned())
        {
            return;
        }

        float speedMod = statusEffects.GetMoveSpeedMod();

        cc.MaxSpeed = startingSpeed * speedMod;

        cc.Move(direction.normalized);

        transform.rotation = Quaternion.Euler(0, (float)yaw, 0);
    }

    private void SetupCamera()
    {
        Debug.Log("Setting up camera");
        var localCamera = Runner.SimulationUnityScene.FindObjectsOfTypeInOrder<FPSCamera>()[0];
        cam = localCamera;

        if (Object.HasInputAuthority)
        {
            Debug.Log("Setting camera target");
            localCamera.SetTarget(this);
            Runner.AddSimulationBehaviour(localCamera);
        }
    }
}
