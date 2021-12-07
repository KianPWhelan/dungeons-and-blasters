using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//To get the obj to move, create an empty parent and give it the script. Then move around the child. (or the other way around)
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
