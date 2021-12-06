using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class InputPoller : MonoBehaviour
{
    [SerializeField]
    private Keybinds playerKeybinds;

    [SerializeField]
    private Keybinds dmKeybinds;

    private FPSCamera fpsCamera;

    [SerializeField]
    private BoolVariable isDungeonMaster;

    private DungeonMasterController dm;

    private float deltaScroll;

    public void Start()
    {
        
    }

    public void Update()
    {
        deltaScroll += Input.mouseScrollDelta.y;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if(isDungeonMaster.runtimeValue)
        {
            SetInput(runner, input, dmKeybinds);
        }

        else
        {
            SetInput(runner, input, playerKeybinds);
        }
    }

    private void SetInput(NetworkRunner runner, NetworkInput input, Keybinds keybinds)
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

        if (keybinds.bindings.TryGetValue(Actions.AltFire, out key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_FIRE_ALT;
        }

        if (keybinds.bindings.TryGetValue(Actions.Select, out key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_SELECT;
        }

        if (keybinds.bindings.TryGetValue(Actions.Multi, out key) && Input.GetKey(key))
        {
            inputData.Buttons |= PlayerInput.BUTTON_MULTI;
        }

        if (!isDungeonMaster.runtimeValue)
        {
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
        }

        else
        {
            inputData.mousePoint = GetMousePoint();
        }

        inputData.deltaScroll = deltaScroll;
        deltaScroll = 0;

        input.Set(inputData);
    }

    private Vector3 GetMousePoint()
    {
        if (dm == null)
        {
            dm = FindObjectOfType<DungeonMasterController>();
        }

        if(dm == null)
        {
            return Vector3.negativeInfinity;
        }

        Ray ray = dm.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
        System.Array.Sort(hits, delegate (RaycastHit hit1, RaycastHit hit2) { return hit1.distance.CompareTo(hit2.distance); });

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "Ground")
            {
                return hit.point;
            }
        }

        return Vector3.negativeInfinity;
    }
}
