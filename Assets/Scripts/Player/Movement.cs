using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody body;

    [Tooltip("Strength of the entity's jump")]
    [SerializeField]
    private FloatVariable jumpPower;

    // Start is called before the first frame update
    public void Start()
    {
        body = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed " + jumpPower.runtimeValue);
            body.AddForce(new Vector3(0, jumpPower.runtimeValue, 0));
        }
    }
}
