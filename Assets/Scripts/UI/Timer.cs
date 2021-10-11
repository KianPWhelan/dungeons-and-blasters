using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private FloatVariable gameTime;

    private Text text;

    public void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    public void FixedUpdate()
    {
        text.text = "Time: " + (int)gameTime.runtimeValue;
    }
}
