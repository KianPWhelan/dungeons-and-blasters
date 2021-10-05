using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Controller : MonoBehaviour
{
    private Movement movement;
    private PlayerCamera playerCam;
    private WeaponHolder weaponHolder;

    public void Start()
    {
        movement = gameObject.GetComponent<Movement>();
        playerCam = gameObject.transform.GetComponentInChildren<PlayerCamera>();
        weaponHolder = gameObject.GetComponent<WeaponHolder>();
    }

    public void Update()
    {
        ProcessInputs();
    }

    public void FixedUpdate()
    {
        ProcessMovement();
    }

    public void GameOver()
    {
        Debug.Log("Game has ended");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    /// <summary>
    /// Check for key presses from 
    /// </summary>
    private void ProcessInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.Jump();   
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Weapon button pressed");
            weaponHolder.UseWeapon(0);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void ProcessMovement()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        movement.Rotate(mouseX, 0.0f);
        playerCam.Rotate(mouseX, mouseY);
        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");
        movement.Move(moveX, moveY);
    }
}
