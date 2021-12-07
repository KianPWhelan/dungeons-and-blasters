using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyManager : MonoBehaviour
{
    public static List<NetworkObject> enemies = new List<NetworkObject>();

    public void Start()
    {
        var temp = FindObjectsOfType<EnemyGeneric>();

        foreach(EnemyGeneric e in temp)
        {
            enemies.Add(e.GetComponent<NetworkObject>());
        }
    }
}
