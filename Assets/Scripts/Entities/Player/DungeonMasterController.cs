using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DungeonMasterController : NetworkBehaviour
{
    private Vector3 velocity;

    [SerializeField]
    private float panSpeed;

    [SerializeField]
    private float zoomStep;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private GameObject panel;

    public Camera cam;

    private UnitSelector unitSelector;

    private float panInitialValue;

    private float startHeight;

    public override void Spawned()
    {
        panInitialValue = panSpeed;
        startHeight = transform.position.y;

        TryGetComponent(out unitSelector);

        if(Object.HasInputAuthority)
        {
            unitSelector.render = true;
            Runner.SimulationUnityScene.FindObjectsOfTypeInOrder<FPSCamera>()[0].gameObject.SetActive(false);
        }

        else
        {
            cam.gameObject.SetActive(false);
            canvas.SetActive(false);
            panel.SetActive(false);
        }
    }

    public override void FixedUpdateNetwork()
    {
        velocity = Vector3.zero;

        if(GetInput(out PlayerInput input))
        {
            if(input.IsDown(PlayerInput.BUTTON_FORWARD))
            {
                velocity += Vector3.forward * panSpeed;
            }

            if(input.IsDown(PlayerInput.BUTTON_BACKWARDS))
            {
                velocity += -Vector3.forward * panSpeed;
            }

            if(input.IsDown(PlayerInput.BUTTON_LEFT))
            {
                velocity += -Vector3.right * panSpeed;
            }

            if(input.IsDown(PlayerInput.BUTTON_RIGHT))
            {
                velocity += Vector3.right * panSpeed;
            }

            if (input.deltaScroll < 0)
            {
                velocity += new Vector3(0f, zoomStep, 0f);
            }

            else if (input.deltaScroll > 0)
            {
                velocity += new Vector3(0f, -zoomStep, 0f);
            }

            //unitSelector.HandleInputs(input);

            transform.position += velocity;

            //panSpeed = panInitialValue + transform.position.y - startHeight;
        }
    }
}
