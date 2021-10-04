using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers
{
    /// <summary>
    /// Gets the closest game object with the desired tag
    /// </summary>
    /// <returns></returns>
    public static GameObject FindClosest(Transform myPosition, string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = myPosition.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public static GameObject FindClosestVisible(Transform myPosition, string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = myPosition.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && CheckLineOfSight(myPosition, go.transform))
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    /// <summary>
    /// Returns true if there is line of sight between the from and to transforms
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static bool CheckLineOfSight(Transform from, Transform to)
    {
        var rayDirection = to.position - from.position;
        RaycastHit hit;
        if (Physics.Raycast(from.position, rayDirection, out hit))
        {
            if (hit.transform == to.transform)
            {
                return true;
            }
        }

        return false;
    }
}
