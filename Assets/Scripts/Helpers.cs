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
}