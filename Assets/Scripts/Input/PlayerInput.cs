using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct PlayerInput : INetworkInput
{
    // Directional controls
    public const uint BUTTON_FORWARD = 1 << 3;
    public const uint BUTTON_BACKWARDS = 1 << 4;
    public const uint BUTTON_LEFT = 1 << 5;
    public const uint BUTTON_RIGHT = 1 << 6;

    // PLAYER CONTROLS
    // Interaction controls
    public const uint BUTTON_USE = 1 << 0;
    public const uint BUTTON_FIRE = 1 << 1;
    public const uint BUTTON_FIRE_ALT = 1 << 2;

    // Additional movement controls
    public const uint BUTTON_JUMP = 1 << 7;

    // DM CONTROLS
    // Interaction controls
    public const uint BUTTON_SELECT = 1 << 8;
    public const uint BUTTON_MULTI = 1 << 9;

    // Enemy movement controls
    public const uint BUTTON_SET_DESTINATION = 1 << 10;

    // All buttons
    public uint Buttons;

    // Camera controls
    public Angle yaw;
    public Angle pitch;
    public float deltaScroll;
    public Vector3 mousePoint;
    public Vector3 mousePosition;

    // Helper functions
    public bool IsUp(uint button)
    {
        return !IsDown(button);
    }

    public bool IsDown(uint button)
    {
        return (Buttons & button) == button;
    }
}
