using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    private Rigidbody body;

    [Tooltip("Strength of the entity's jump")]
    [SerializeField]
    private FloatVariable jumpPower;

    [Tooltip("Rotation speed")]
    [SerializeField]
    private FloatVariable rotationSpeed;

    [Tooltip("Movement speed")]
    [SerializeField]
    private FloatVariable moveSpeed;

    // Rotation values of object
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    public void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Applies an upward force on the game object's rigid body based on jump power
    /// </summary>
    public void Jump()
    {
        body.AddForce(new Vector3(0, jumpPower.runtimeValue, 0));
    }

    /// <summary>
    /// Translates the rigid body based on the provided input
    /// </summary>
    public void Move(float x, float y)
    {
        var transform = body.transform;
        body.MovePosition(transform.position + (transform.forward * y * moveSpeed.runtimeValue) + (transform.right * x * moveSpeed.runtimeValue));
    }

    /// <summary>
    /// Rotates the object based on the X and Y values and rotation speed
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Rotate(float x, float y)
    {
        yRotation += x * rotationSpeed.runtimeValue;
        xRotation -= y * rotationSpeed.runtimeValue;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        body.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
    }
}
