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

    private float timer1;

    private float timer2;

    private bool timeout;

    private bool stopping;

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
        Debug.Log("numSums: " + numSums);
    }

    public void SetDestination(Vector3 destination)
    {
        Debug.Log("Destination Set: " + destination);
        this.destination = destination;
    }

    public void SendAllUnitsToNextPoint(EnemyGeneric exception)
    {
        Debug.Log("Here");

        foreach(EnemyGeneric e in units)
        {
            if(e != exception)
            {
                e.ProcessQueue(false);
            }
        }
    }

    public void Start()
    {
        timer2 = Time.time;
    }

    public void Process()
    { 
        //Debug.Log("Processing");

        foreach (EnemyGeneric e in units)
        {
            sum += e.transform.position;
        }

        //Debug.Log("Sum " + sum);
        Vector3 average = sum / units.Count;
        average = new Vector3(average.x, average.y, average.z);
        //Debug.Log("Average " + average);
        //Debug.Log("Destination " + destination);
        //Debug.Log(average + " " + destination + "  " + (1 + (0.00001 * Mathf.Pow(units.Count, 2))));
        var dist = Vector3.Distance(average, destination);
        //Debug.Log("Distance: " + dist);

        if (lastAverage.x == Mathf.NegativeInfinity)
        {
            lastAverage = average;
            //Debug.Log("Last Average " + lastAverage);
        }

        if (Time.time - timer2 >= 0.5)
        {
            //Debug.Log("dist " + Vector3.Distance(average, lastAverage));
            distances.Add(Vector3.Distance(average, lastAverage));
            lastAverage = average;
            //Debug.Log("Last Average " + lastAverage);
        timer2 = Time.time;
        }

        if (distances.Count >= 5)
        {
            float greatestDifference = 0;

            foreach (float d in distances)
            {
                if (d > greatestDifference)
                {
                    greatestDifference = d;
                }
            }

            distances = new List<float>();
            Debug.Log(greatestDifference + " " + Mathf.Max(0.5f / Mathf.Pow(1.005f, units.Count), 0.3f));

            if (greatestDifference < Mathf.Max(0.5f / Mathf.Pow(1.005f, units.Count), 0.3f))
            {
                timer1 = Time.time;
                stopping = true;
            }
        }

        if (dist < 1 + (0.00001 * Mathf.Pow(units.Count, 2)) || timeout)
        {
            //Debug.Log("Timeout " + timeout);
            timeout = false;
            stopping = false;
            lastAverage = Vector3.negativeInfinity;
            //Debug.Log("Done");    

            foreach (EnemyGeneric e in units)
            {
                e.ClearQueue();
                e.ClearPath();
                SquadManager.instance.squadsToDelete.Add(this);
            }
        }


        sum = Vector3.zero;
        numSums = 0;

        if(Time.time - timer1 > 2 && stopping)
        {
            timeout = true;
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
