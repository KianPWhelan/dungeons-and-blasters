using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float_Rotation_Script : MonoBehaviour
{
    public float amp;
    public float freq;
    Vector3 initPos;
    [SerializeField] private Vector3 _rotation;

    private void Start()
    {
        initPos = transform.position;
    }

    private void Update()
    {
        transform.position = new Vector3(initPos.x, Mathf.Sin(Time.time * freq) * amp + initPos.y, 0);
        transform.Rotate(_rotation * Time.deltaTime);
    }
}