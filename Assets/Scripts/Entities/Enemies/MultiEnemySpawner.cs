using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Com.OfTomorrowInc.DMShooter;

public class MultiEnemySpawner : EnemyGeneric
{
    public NetworkObject enemyToSpawn;
    public int numToSpawn;

    // Start is called before the first frame update
    public override void Spawned()
    {
        if(!Object.HasStateAuthority)
        {
            return;
        }

        //string name = enemyToSpawn.name;
        List<EnemyGeneric> newEnemies = new List<EnemyGeneric>();

        for(int i = 0; i < numToSpawn; i++)
        {
            var enemy = Runner.Spawn(enemyToSpawn, transform.position, Quaternion.identity);
            newEnemies.Add(enemy.GetComponent<EnemyGeneric>());
            EnemyManager.enemies.Add(enemy);
        }

        StartCoroutine(DeclutterNextFrame(newEnemies));
    }

    private IEnumerator DeclutterNextFrame(List<EnemyGeneric> enemies)
    {
        yield return new WaitForFixedUpdate();
        foreach(EnemyGeneric e in enemies)
        {
            int offset = Random.Range(0, 4);
            if(offset == 0)
            {
                e.MoveTo(transform.position + new Vector3(0f, 0f, e.agent.stoppingDistance + 0.05f));
            }

            else if(offset == 1)
            {
                e.MoveTo(transform.position + new Vector3(0f, 0f, -e.agent.stoppingDistance - 0.05f));
            }

            else if(offset == 2)
            {
                e.MoveTo(transform.position + new Vector3(e.agent.stoppingDistance + 0.05f, 0f, 0f));
            }

            else if(offset == 3)
            {
                e.MoveTo(transform.position + new Vector3(-e.agent.stoppingDistance - 0.05f, 0f, 0f));
            }
        }

        Runner.Despawn(Object);
    }
}
