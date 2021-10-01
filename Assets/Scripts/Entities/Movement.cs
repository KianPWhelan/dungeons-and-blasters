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

    [Tooltip("Number of jumps")]
    [SerializeField]
    private IntVariable numberOfJumps;

    [Tooltip("Rotation speed")]
    [SerializeField]
    private FloatVariable rotationSpeed;

    [Tooltip("Movement speed")]
    [SerializeField]
    private FloatVariable moveSpeed;

    // Current jumps used
    private int jumpsUsed = 0;

    // Rotation values of object
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    // Previous position
    private Vector3 previousPosition;

    public void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Applies an upward force on the game object's rigid body based on jump power
    /// </summary>
    public void Jump()
    {
        if (jumpsUsed < numberOfJumps.runtimeValue)
        {
            body.AddForce(new Vector3(0, jumpPower.runtimeValue, 0));
            jumpsUsed += 1;
        }
    }

    /// <summary>
    /// Translates the rigid body based on the provided input
    /// </summary>
    public void Move(float x, float y)
    {
        var transform = body.transform;
        var desiredPosition = transform.position + (transform.forward * y * moveSpeed.runtimeValue) + (transform.right * x * moveSpeed.runtimeValue);
        var direction = desiredPosition - transform.position;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, direction.magnitude, ~0, QueryTriggerInteraction.Ignore))
            body.MovePosition(desiredPosition);
        else
        {
            Debug.Log("Ray hit");
            body.MovePosition(hit.point);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            jumpsUsed = 0;
        }
    }
}
