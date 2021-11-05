using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMakerCamera : MonoBehaviour
{
    [Tooltip("Panning speed of the master")]
    [SerializeField]
    public FloatVariable panSpeed;

    [Tooltip("Zooming speed of the master")]
    [SerializeField]
    public FloatVariable zoomStep;

    [HideInInspector]
    public Camera camera;

    private Rigidbody body;

    private float startHeight;

    // Start is called before the first frame update
    public void Start()
    {
        camera = gameObject.GetComponent<Camera>();
        body = GetComponent<Rigidbody>();
        startHeight = transform.position.y;
    }

    // Update is called once per frame
    public void Update()
    {
        var scroll = Input.mouseScrollDelta.y;

        if (scroll < 0)
        {
            body.transform.position = body.transform.position + new Vector3(0f, zoomStep.runtimeValue, 0f);
        }

        else if (scroll > 0)
        {
            body.transform.position = body.transform.position + new Vector3(0f, -zoomStep.runtimeValue, 0f);
        }
        var change = panSpeed.initialValue + body.transform.position.y - startHeight;
        // Debug.Log(change);
        panSpeed.runtimeValue = change;
    }

    public void FixedUpdate()
    {
        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");
        body.velocity = new Vector3(moveX * panSpeed.runtimeValue, 0f, moveY * panSpeed.runtimeValue);
    }
}
