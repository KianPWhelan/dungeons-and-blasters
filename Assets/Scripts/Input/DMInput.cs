using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct DMInput : INetworkInput
{
    // Interaction controls
    public const uint BUTTON_SELECT = 1 << 0;
    public const uint BUTTON_FIRE = 1 << 1;
    public const uint BUTTON_FIRE_ALT = 1 << 2;

    // Directional controls
    public const uint BUTTON_FORWARD = 1 << 3;
    public const uint BUTTON_BACKWARDS = 1 << 4;
    public const uint BUTTON_LEFT = 1 << 5;
    public const uint BUTTON_RIGHT = 1 << 6;

    // All buttons
    public uint Buttons;

    // Camera controls
    public Angle yaw;
    public Angle pitch;
    public float deltaScroll;

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
