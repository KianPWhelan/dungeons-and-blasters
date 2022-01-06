using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemySpawner : NetworkBehaviour
{
    [Tooltip("If true, spawner will spawn the units in a sequence instead of randomly")]
    public bool spawnSequenced;

    [Tooltip("Inclusive")]
    public float spawnMinDelay;

    [Tooltip("Inclusive")]
    public float spawnMaxDelay;

    public float spawnMinRange;

    public float spawnMaxRange;

    public List<SpawningInfo> units = new List<SpawningInfo>();

    private float spawnDelay;

    private float startTime;

    private int sequenceIndex;

    [SerializeField]
    private List<NetworkObject> weightedSpawnList = new List<NetworkObject>();

    [System.Serializable]
    public class SpawningInfo
    {
        public NetworkObject unit;

        [Range(0, 99)]
        public int weight;
    }

    public override void Spawned()
    {
        if(!Object.HasStateAuthority)
        {
            return;
        }

        EnemyManager.spawners.Add(Object);
        SetSpawnDelay();
        GenerateWeightedSpawnList();
    }

    public override void FixedUpdateNetwork()
    {
        if(!Object.HasStateAuthority)
        {
            return;
        }

        CheckAndPerformSpawn();
    }

    private void SetSpawnDelay()
    {
        spawnDelay = Random.Range(spawnMinDelay, spawnMaxDelay);
        startTime = Time.time;
    }

    private void GenerateWeightedSpawnList()
    {
        foreach(SpawningInfo unit in units)
        {
            for(int i = 0; i < unit.weight; i++)
            {
                weightedSpawnList.Add(unit.unit);
            }
        }
    }

    private void CheckAndPerformSpawn()
    {
        if(Time.time - startTime >= spawnDelay)
        {
            if(spawnSequenced)
            {
                SpawnUnitSequenced();
            }

            else
            {
                SpawnUnitUnsequenced();
            }
            
            SetSpawnDelay();
        }
    }

    private void SpawnUnitSequenced()
    {
        var unit = units[sequenceIndex].unit;
        var location = GetSpawnLocation();
        EnemyManager.enemies.Add(Runner.Spawn(unit, location));
        sequenceIndex++;

        if(sequenceIndex >= units.Count)
        {
            sequenceIndex = 0;
        }
    }

    private void SpawnUnitUnsequenced()
    {
        var unit = GetUnitToSpawn();
        var location = GetSpawnLocation();
        EnemyManager.enemies.Add(Runner.Spawn(unit, location));
    }

    private NetworkObject GetUnitToSpawn()
    {
        int index = Random.Range(0, weightedSpawnList.Count);
        Debug.Log("Chooseing unit at index " + index + " which is " + weightedSpawnList[index].name);
        return weightedSpawnList[index];
    }

    private Vector3 GetSpawnLocation()
    {
        float range = Random.Range(spawnMinRange, spawnMaxRange);
        Vector3 location = transform.position + Vector3.forward * range;
        Vector3 diff = location - transform.position;
        float degrees = Random.Range(0, 360f);
        diff = Helpers.RotateVectorByDegrees(diff, degrees, Vector3.up);
        location = transform.position + diff;
        return location;
    }
}
