using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGenericNoNavmesh : EnemyGeneric
{
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out statusEffects);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //target = Helpers.FindClosest(gameObject.transform, targetType);
        //aiModule.Tick(gameObject, target, agent, movement);
    }

    private void Update()
    {
        // target = Helpers.FindClosest(gameObject.transform, targetType);
        aiModule.Tick(gameObject, null, null, null, null);
    }
}
