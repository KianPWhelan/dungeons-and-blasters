using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Squad
{
    public List<EnemyGeneric> units = new List<EnemyGeneric>();

    public Vector3 destination;

    private Vector3 sum;

    private int numSums;

    private Vector3 lastAverage = Vector3.negativeInfinity;

    private List<float> distances = new List<float>();

    private float timer2;

    public void AddUnit(EnemyGeneric unit)
    {
        units.Add(unit);
    }

    public void RemoveUnit(EnemyGeneric unit)
    {
        units.Remove(unit);
    }

    public void SumAndIncrement(Vector3 change)
    {
        sum += change;
        numSums++;
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }

    public void Process()
    {
        if(numSums >= units.Count)
        {
            Debug.Log("Processing");
            //Vector3 average = sum / units.Count;
            //average = new Vector3(average.x, average.y - 1.1f, average.z);
            ////Debug.Log(average + " " + destination + "  " + (1 + (0.00001 * Mathf.Pow(units.Count, 2))));
            //var dist = Vector3.Distance(average, destination);

            //if (lastAverage.x == Mathf.NegativeInfinity)
            //{
            //    lastAverage = average;
            //    //Debug.Log("Last Average " + lastAverage);
            //}

            //if (Time.time - timer2 >= 0.5)
            //{
            //    //Debug.Log("dist " + Vector3.Distance(average, lastAverage));
            //    distances.Add(Vector3.Distance(average, lastAverage));
            //    lastAverage = average;
            //    //Debug.Log("Last Average " + lastAverage);
            //    timer2 = Time.time;
            //}
        }
    }

    public void Delete()
    {
        foreach(EnemyGeneric e in units)
        {
            e.squad = null;
        }
    }
}
