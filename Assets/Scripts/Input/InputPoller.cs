using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class InputPoller : MonoBehaviour
{
    [SerializeField]
    private Keybinds keybinds;

    private FPSCamera fpsCamera;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var inputData = new PlayerInput();

        if (keybinds.bindings == null)
        {
            keybinds.SetKeybinds();
        }

        if (keybinds.bindings.TryGetValue(Actions.Forward, out KeyCode key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_FORWARD;
        }

        if (keybinds.bindings.TryGetValue(Actions.Backwards, out key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_BACKWARDS;
        }

        if (keybinds.bindings.TryGetValue(Actions.Left, out key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_LEFT;
        }

        if (keybinds.bindings.TryGetValue(Actions.Right, out key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_RIGHT;
        }

        if (keybinds.bindings.TryGetValue(Actions.Jump, out key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_JUMP;
        }

        if (keybinds.bindings.TryGetValue(Actions.Fire, out key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_FIRE;
        }

        if (fpsCamera == null)
        {
            fpsCamera = runner.SimulationUnityScene.FindObjectsOfTypeInOrder<FPSCamera>()[0];
        }

        else
        {
            var yawPitch = fpsCamera.ConsumeYawPitch();
            inputData.yaw = yawPitch.Item1;
            inputData.pitch = yawPitch.Item2;
        }

        input.Set(inputData);
    }
}
