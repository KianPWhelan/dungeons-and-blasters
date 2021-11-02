using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConstants : ScriptableObject
{
    [SerializeField]
    private float roomSpawnEnemiesDelayField;

    public static float roomSpawnEnemiesDelay;

    public void OnEnable()
    {
        roomSpawnEnemiesDelay = roomSpawnEnemiesDelayField;
    }
}
