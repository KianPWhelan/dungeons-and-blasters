using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoBake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var surfaces = GetComponents<NavMeshSurface>();

        foreach(NavMeshSurface s in surfaces)
        {
            s.BuildNavMesh();
        }
    }
}
