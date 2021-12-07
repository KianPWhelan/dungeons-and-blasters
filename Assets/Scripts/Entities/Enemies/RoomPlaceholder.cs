using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Com.OfTomorrowInc.DMShooter;

public class RoomPlaceholder : MonoBehaviour
{
    public NetworkObject enemy;

    public float cost;

    public int numToSpawn;

    private NetworkRunner runner;

    public void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();
    }

    public void SpawnEnemies()
    {
        if(!runner.IsServer)
        {
            return;
        }

        for(int i = 0; i < numToSpawn; i++)
        {
            var newEnemy = runner.Spawn(enemy, transform.position, Quaternion.identity);
            EnemyManager.enemies.Add(newEnemy);
        }

        Destroy(gameObject);
    }
}
