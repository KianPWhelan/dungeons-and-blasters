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

    private EnemyManager enemyManager;

    public void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();
        enemyManager = FindObjectOfType<EnemyManager>();
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
            enemyManager.AddEnemy(newEnemy);
        }

        Destroy(gameObject);
    }
}
