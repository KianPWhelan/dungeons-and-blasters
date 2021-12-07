using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static List<GameObject> enemies = new List<GameObject>();

    public void Start()
    {
        var temp = FindObjectsOfType<EnemyGeneric>();

        foreach(EnemyGeneric e in temp)
        {
            enemies.Add(e.gameObject);
        }
    }
}
