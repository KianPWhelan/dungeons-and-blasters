using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Tooltip("Rotation speed")]
    [SerializeField]
    private FloatVariable rotationSpeed;

    private Transform parent;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    public void Start()
    {
        parent = gameObject.transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    public void Update()
    {
        gameObject.transform.position = parent.position;
    }

    /// <summary>
    /// Rotates the camera based on the X and Y values and rotation speed
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Rotate(float x, float y)
    {
        yRotation += x * rotationSpeed.runtimeValue;
        xRotation -= y * rotationSpeed.runtimeValue;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        gameObject.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
    }
}
