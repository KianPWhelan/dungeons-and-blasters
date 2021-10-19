using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyData
{
    public string name;
    public float cooldown;
    public float slotSize;
    public float[] ranges;
    public float health;
    public float speed;
    public float angularSpeed;
    public float acceleration;

    public static EnemyData GetEnemyData(GameObject obj)
    {
        try
        {
            EnemyGeneric e = obj.GetComponent<EnemyGeneric>();
            NavMeshAgent n = obj.GetComponent<NavMeshAgent>();
            Health h = obj.GetComponent<Health>();

            EnemyData data = new EnemyData();

            data.name = obj.name;
            data.cooldown = e.cooldown;
            data.slotSize = e.slotSize;
            data.ranges = e.ranges.ToArray();
            data.health = h.GetHealth();
            data.speed = n.speed;
            data.angularSpeed = n.angularSpeed;
            data.acceleration = n.acceleration;

            return data;
        }

        catch
        {
            Debug.LogError("Enemy Data Parse Error: Does the object have an EnemyGeneric, Health, and NavMeshAgent component?");
            return null;
        }
    }

    public static void LoadEnemyData(GameObject obj, EnemyData data)
    {
        try
        {
            EnemyGeneric e = obj.GetComponent<EnemyGeneric>();
            NavMeshAgent n = obj.GetComponent<NavMeshAgent>();
            Health h = obj.GetComponent<Health>();

            e.cooldown = data.cooldown;
            e.slotSize = (int)data.slotSize;
            e.ranges = new List<float>(data.ranges);
            h.SetHealth(data.health);
            n.speed = data.speed;
            n.angularSpeed = data.angularSpeed;
            n.acceleration = data.acceleration;
        }

        catch
        {
            Debug.LogError("Enemy Data Parse Error: Does the object have an EnemyGeneric, Health, and NavMeshAgent component?");
        }
    }
}