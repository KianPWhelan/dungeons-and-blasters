using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueIdGenerator : ScriptableObject
{
    [SerializeField]
    private string result;

    public void GenerateGUID()
    {
        result = Guid.NewGuid().ToString();
    }
}
