using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoBake : MonoBehaviour
{
    public bool bakeOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        if(!bakeOnStart)
        {
            return;
        }

        BakeAll();
    }

    public void BakeAll()
    {
        var surfaces = GetComponents<NavMeshSurface>();

        foreach (NavMeshSurface s in surfaces)
        {
            s.BuildNavMesh();
        }
    }
}
