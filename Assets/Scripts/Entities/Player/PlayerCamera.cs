using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform parent;

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
        gameObject.transform.rotation = parent.rotation;
    }
}
