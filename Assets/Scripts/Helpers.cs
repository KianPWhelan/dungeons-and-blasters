using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            if (curDistance < distance && go.transform != myPosition)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    /// <summary>
    /// Gets the closest game object with the desired tag
    /// </summary>
    /// <returns></returns>
    public static GameObject FindClosest(Vector3 myPosition, string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = myPosition;
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

    /// <summary>
    /// Gets the closest game object with the desired tag which is visible
    /// </summary>
    /// <returns></returns>
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
            if (curDistance < distance && go.transform != myPosition && CheckLineOfSight(myPosition, go.transform, 100000))
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

        /// <summary>
        /// Gets the closest game object with the desired tag
        /// </summary>
        /// <returns></returns>
        public static List<GameObject> FindAllInRange(Transform myPosition, float range, string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        Vector3 position = myPosition.position;
        List<GameObject> valid = new List<GameObject>();

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            // Debug.Log("Distance: " + curDistance);

            if(curDistance <= range)
            {
                // Debug.Log("Adding tile");
                valid.Add(go);
            }
        }

        return valid;
    }

    /// <summary>
    /// Returns true if there is line of sight between the from and to transforms
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static bool CheckLineOfSight(Transform from, Transform to, float visionRange)
    {
        if(Vector3.Distance(from.position, to.position) > visionRange)
        {
            return false;
        }

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

    /// <summary>
    /// Returns the normal of a mesh collider at the raycast hit point
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    public static Vector3 GetMeshColliderNormal(RaycastHit hit)
    {
        Collider collider = hit.collider;
        Mesh mesh = collider.gameObject.GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;


        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];
        Vector3 baryCenter = hit.barycentricCoordinate;
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        interpolatedNormal.Normalize();
        interpolatedNormal = hit.transform.TransformDirection(interpolatedNormal);
        return interpolatedNormal;
    }

    /// <summary>
    /// Executes the provided method after the provided time has passed. Use inside of a coroutine
    /// </summary>
    /// <param name="action"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static IEnumerator Timeout(Action action, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        action();
    }

    /// <summary>
    /// Returns the number of occurrences of the specific object in the specific list
    /// </summary>
    /// <param name="list"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static int NumOccurences(List<object> list, object obj)
    {
        int count = 0;

        foreach(object o in list)
        {
            if(o == obj)
            {
                count++;
            }
        }

        return count;
    }
}