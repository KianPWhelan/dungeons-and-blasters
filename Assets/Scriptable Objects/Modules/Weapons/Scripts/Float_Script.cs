using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Cant find out why i cant move the cubes in game. Maybe someone could help me with that
public class Float_Script : MonoBehaviour
{
    public float amp;
    public float freq;
    Vector3 initPos;

    private void Start()
    {
        initPos = transform.position;
    }

    private void Update()
    {
        transform.position = new Vector3(initPos.x, Mathf.Sin(Time.time * freq) * amp + initPos.y, 0);
    }
}
